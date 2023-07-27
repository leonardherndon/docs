using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Huenity
{
    public class HuenityController : MonoBehaviour
    {
        private string _connectionString;

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Bridge
        private int socketScanTime = 2000;

        readonly IPAddress multicastAddress = IPAddress.Parse("239.255.255.250");
        const int multicastPort = 1900;
        const int unicastPort = 1901;

        const string messageHeader = "M-SEARCH * HTTP/1.1";
        const string messageHost = "HOST: 239.255.255.250:1900";
        const string messageMan = "MAN: \"ssdp:discover\"";
        const string messageMx = "MX: 8";
        const string messageSt = "ST:SsdpSearch:all";

        readonly byte[] broadcastMessage = Encoding.UTF8.GetBytes(
            string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{0}",
                          "\r\n",
                          messageHeader,
                          messageHost,
                          messageMan,
                          messageMx,
                          messageSt));

        private List<string> _discoveredDevices = new List<string>();

        /// <summary>
        /// Generates a whitelisted username on the bridge for this app to use.
        /// Will return an error stating that the user needs to press the link button on the bridge if it hasn't been done yet.
        /// </summary>
        /// <param name="bridgeAddress">IP address of the bridge</param>
        /// <param name="appName">App name used as first half of the whitelist key generation</param>
        /// <param name="deviceID">Unique device identifier used as the second have of the whitelist key generation</param>
        /// <param name="username">Callback value that will return either the generated username or error message</param>        
        public IEnumerator ConnectBridge(string bridgeAddress, string appName, string deviceID, Action<string> username)
        {
            WhitelistRequest connection = new WhitelistRequest() { devicetype = appName + "#" + deviceID };
            string whitelistedUsername = string.Empty;
            string errorMessage = string.Empty;

            var timeout = 15;
            while (timeout > 0 && whitelistedUsername == string.Empty)
            {
                StartCoroutine(HueRequestHelper.SendRequestAsync(bridgeAddress, "/api", UnityWebRequest.kHttpVerbPOST, callback =>
                {
                    var response = JSON.Parse(callback);
                    if (response != null)
                    {
                        if (response[0]["error"]["description"] != null)
                        {
                            errorMessage = response[0]["error"]["description"];
                            Debug.Log(errorMessage);
                        }
                        else if (response[0]["success"]["username"] != null)
                        {
                            whitelistedUsername = response[0]["success"]["username"];
                            username(whitelistedUsername);
                        }
                    }
                    else
                    {
                        errorMessage = "unable to connect to bridge";
                        timeout = -1;
                    }
                }, JsonUtility.ToJson(connection)));

                yield return new WaitForSecondsRealtime(2f);
                timeout--;
            }

            if (whitelistedUsername == string.Empty)
            {
                username("error: " + errorMessage);
            }
        }

        /// <summary>
        /// Scans the local network for SSDP devices and identifies the hue bridge.
        /// </summary>
        /// <param name="response">Callback value that will return the IP address of the hue bridge</param>
        public void FindBridge(Action<string> response)
        {
            Debug.Log("Scanning for SSDP devices");
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, unicastPort));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddress, IPAddress.Any));
                var thd = new Thread(() => GetSocketResponse(socket));
                socket.SendTo(broadcastMessage, 0, broadcastMessage.Length, SocketFlags.None, new IPEndPoint(multicastAddress, multicastPort));
                thd.Start();
                Thread.Sleep(socketScanTime);
                socket.Close();
            }

            StartCoroutine(IdentifyBridge(callback =>
            {
                response(callback);
            }));
        }

        private void GetSocketResponse(Socket socket)
        {
            while (true)
            {
                var response = new byte[8000];
                EndPoint ep = new IPEndPoint(IPAddress.Any, multicastPort);
                socket.ReceiveFrom(response, ref ep);

                try
                {
                    var receivedString = Encoding.UTF8.GetString(response);

                    var location = receivedString.Substring(receivedString.IndexOf("LOCATION:", System.StringComparison.Ordinal) + 9);
                    receivedString = location.Substring(0, location.IndexOf("\r", System.StringComparison.Ordinal)).Trim();

                    _discoveredDevices.Add(receivedString);
                }
                catch
                {
                    //Not a UTF8 string, ignore this response.
                }
            }
        }

        private IEnumerator IdentifyBridge(Action<string> response)
        {
            Debug.Log("Identifying Hue bridge");
            var endpoints = _discoveredDevices.Where(s => s.EndsWith("/description.xml")).ToList();
            string foundIp = string.Empty;

            foreach (var endpoint in endpoints)
            {
                var ip = endpoint.Replace("/description.xml", "");

                var webRequest = new UnityWebRequest(ip);

                DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
                webRequest.downloadHandler = downloadHandler;

                yield return webRequest.Send();

                if (webRequest.isNetworkError)
                {
                    Debug.Log(webRequest.error);
                }
                else
                {
                    if (downloadHandler != null && downloadHandler.text.Contains("hue"))
                    {
                        if (ip != foundIp)
                        {
                            foundIp = ip;
                            response(ip);
                        }
                    }
                }
            }

            if (foundIp == string.Empty)
            {
                response("error: unable to locate hue bridge");
            }
        }

        public void DeleteUser(string username)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/config/whitelist/" + username, UnityWebRequest.kHttpVerbDELETE, null));
        }
        #endregion

        #region Lights

        /// <summary>
        /// Retrieves the list of all lights connected to the hue bridge
        /// </summary>
        /// <param name="allLights">Callback value that will return a List<HueLight> containing all connected lights</param>
        public void GetAllLights(Action<List<HueLight>> allLights)
        {
            List<HueLight> foundHueLights = new List<HueLight>();

            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/lights/", UnityWebRequest.kHttpVerbGET, callback =>
            {
                var response = JSON.Parse(callback);
                for (int i = 0; i < response.Count; i++)
                {
                    foundHueLights.Add(new HueLight()
                    {
                        id = i + 1,
                        name = response[i]["name"],
                    });
                }
                allLights(foundHueLights);
            }));
        }

        /// <summary>
        /// Retrieves the list of new lights discovered by the SearchForNewLights method.
        /// </summary>
        /// <param name="newLights">Callback value that will return a List<HueLight> containing all new lights</param>
        public void GetNewLights(Action<List<HueLight>> newLights)
        {
            List<HueLight> newHueLights = new List<HueLight>();

            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/lights/new/", UnityWebRequest.kHttpVerbGET, callback =>
            {
                var response = JSON.Parse(callback);
                for (int i = 0; i < response.Count; i++)
                {
                    newHueLights.Add(new HueLight()
                    {
                        name = response[i],
                        id = response[i]["name"],
                    });
                }
                newLights(newHueLights);
            }));
        }

        /// <summary>
        /// Initiates the bridge to search for new lights.
        /// </summary>
        public void SearchForNewLights()
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/lights/", UnityWebRequest.kHttpVerbPOST, null));
        }

        /// <summary>
        /// Sets the state of the specified light.
        /// </summary>
        /// <param name="lightID">Light to set the state for</param>
        /// <param name="lightState">State to set the light to</param>
        public void SetLightState(int lightID, HueLightState lightState)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/lights/" + lightID + "/state", UnityWebRequest.kHttpVerbPUT, null, JSONStateFormatter(lightState)));
        }

        /// <summary>
        /// Gets the attributes and state of the specified light.
        /// </summary>
        /// <param name="lightID">Light to get details for</param>
        /// <param name="light">Callback value used to return the HueLight</param>
        public void GetLight(int lightID, Action<HueLight> light)
        {
            var hueLightState = new HueLightState();
            var hueLight = new HueLight() { id = lightID, state = hueLightState };
            
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/lights/" + lightID, UnityWebRequest.kHttpVerbGET, callback => {
                var response = JSON.Parse(callback);

                hueLight.name = response["name"];
                hueLight.type = response["type"];
                hueLight.modelid = response["modelid"];
                hueLight.swversion = response["swversion"];
                hueLightState.on = response["state"]["on"];
                hueLightState.hue = response["state"]["hue"];
                hueLightState.effect = (Effect)Enum.Parse(typeof(Effect), response["state"]["effect"]);
                hueLightState.alert = (Alert)Enum.Parse(typeof(Alert), response["state"]["alert"]);
                hueLightState.bri = response["state"]["bri"];
                hueLightState.sat = response["state"]["sat"];
                hueLightState.ct = response["state"]["ct"];
                hueLightState.reachable = response["state"]["reachable"];
                hueLightState.colormode = response["state"]["colormode"];

                light(hueLight);
            }));
        }

        /// <summary>
        /// Update attributes of the specified light
        /// </summary>
        /// <param name="light">HueLight to be updated with assigned values</param>
        public void UpdateLight(HueLight light)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/lights/" + light.id, UnityWebRequest.kHttpVerbPUT, null, JsonUtility.ToJson(light)));
        }

        /// <summary>
        /// Remove the specified light from the hue bridge.
        /// </summary>
        /// <param name="light">Light to be removed</param>
        public void DeleteLight(HueLight light)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/lights/" + light.id, UnityWebRequest.kHttpVerbDELETE, null));
        }
        #endregion

        #region Groups

        /// <summary>
        /// Retrieves the list of all groups existing on the hue bridge
        /// </summary>
        /// <param name="allGroups">Callback value that will return a List<HueGroup> containing all groups</param>
        public void GetAllGroups(Action<List<HueGroup>> allGroups)
        {
            List<HueGroup> foundHueGroups = new List<HueGroup>();

            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/groups/", UnityWebRequest.kHttpVerbGET, callback =>
            {
                var response = JSON.Parse(callback);
                for (int i = 0; i < response.Count; i++)
                {
                    var group = new HueGroup()
                    {
                        id = response[i],
                        name = response[i]["name"]
                    };

                    for (int j = 0; j < response["lights"].Count; j++)
                    {
                        group.lights[j] = response[i]["lights"][j];
                    }

                    foundHueGroups.Add(group);
                }
                allGroups(foundHueGroups);
            }));
        }

        /// <summary>
        /// Creates a new group on the hue bridge containing the specified lights.
        /// </summary>
        /// <param name="group">Group to be created</param>
        /// <param name="groupID">Callback value that will return the ID of the created group</param>
        public void CreateGroup(HueGroup group, Action<string> groupID)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/groups/", UnityWebRequest.kHttpVerbPOST, callback =>
            {
                var response = JSON.Parse(callback);
                if (response != null)
                {
                    groupID(response[0]["success"]["id"]);
                }

            }, JsonUtility.ToJson(group)));
        }

        /// <summary>
        /// Update attributes of the specified light
        /// </summary>
        /// <param name="group">HueGroup to be updated with assigned values</param>
        public void UpdateGroup(HueGroup group)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/groups/" + group.id, UnityWebRequest.kHttpVerbPUT, null, JsonUtility.ToJson(group)));
        }

        /// <summary>
        /// Gets the attributes and state of the specified group.
        /// </summary>
        /// <param name="groupID">Group to get the details for</param>
        /// <param name="group">Callback value to return the HueGroup</param>
        public void GetGroup(string groupID, Action<HueGroup> group)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/groups/" + groupID, UnityWebRequest.kHttpVerbGET, groupCallback =>
            {
                var response = JSON.Parse(groupCallback);

                HueGroup returnedGroup = new HueGroup();
                returnedGroup.name = response["name"];
                returnedGroup.lights = new string[response["lights"].Count];

                for (int i = 0; i < response["lights"].Count; i++)
                {
                    returnedGroup.lights[i] = response["lights"][i];
                }

                group(returnedGroup);

            }, JsonUtility.ToJson(group)));
        }

        /// <summary>
        /// Set the state of a specified group.
        /// </summary>
        /// <param name="groupID">Group to set the state for</param>
        /// <param name="lightState">State to be set on the group</param>
        public void SetGroupState(int groupID, HueLightState lightState)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/gropups/" + groupID + "/action", UnityWebRequest.kHttpVerbPUT, null, JSONStateFormatter(lightState)));
        }

        /// <summary>
        /// Remove a group from the hue bridge
        /// </summary>
        /// <param name="group">Group to be removed</param>
        public void DeleteGroup(HueGroup group)
        {
            StartCoroutine(HueRequestHelper.SendRequestAsync(_connectionString, "/groups/" + group.id, UnityWebRequest.kHttpVerbDELETE, null));
        }
        #endregion

        #region Helpers
        private string JSONStateFormatter(HueLightState lightState)
        {
            //{"on":true,"sat":0,"bri":0,"hue":0,"alert":"","effect":""}

            var json = "{";

            if (lightState.on != null)
            {
                json += "\"on\":" + lightState.on.ToString().ToLower() + ",";
            }

            if (lightState.xy != null)
            {
                json += "\"xy\":[" + lightState.xy[0] + ", " + lightState.xy[1] + "]" + ",";
            }

            if (lightState.sat != null)
            {
                json += "\"sat\":" + lightState.sat + ",";
            }

            if (lightState.bri != null)
            {
                json += "\"bri\":" + lightState.bri + ",";
            }

            if (lightState.hue != null)
            {
                json += "\"hue\":" + lightState.hue + ",";
            }

            if (lightState.transitiontime != null)
            {
                json += "\"transitiontime\":" + lightState.transitiontime + ",";
            }

            if (lightState.alert != null)
            {
                json += "\"alert\":\"" + lightState.alert + "\"" + ",";
            }

            if (lightState.effect != null)
            {
                json += "\"effect\":\"" + lightState.effect + "\"" + ",";
            }

            json += "}";

            json = json.Remove(json.LastIndexOf(","), 1);

            return json;
        }
        #endregion
    }
}