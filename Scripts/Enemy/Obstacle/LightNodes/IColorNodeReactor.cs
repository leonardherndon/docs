using MoreMountains.Feedbacks;

public interface IColorNodeReactor
{
    IColorNodeGroup NodeGroup { get; set; }
    MMFeedbacks SuccessFeedback { get; set; }

    void FireFeedback();
}