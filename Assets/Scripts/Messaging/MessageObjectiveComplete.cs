using UnityEngine;

/// <summary>
/// Sent on unit selection.
/// </summary>
public class MessageObjectiveComplete: Message {
	/// <summary>
	/// Records that an objective was completed
	/// </summary>
	/// <param name="aSource">
	/// A <see cref="GameObject"/>
	/// </param>
	public MessageObjectiveComplete(GameObject aSource) : base(aSource, Dispatcher.OBJECTIVE_COMPLETE) {
		base.Send();
	}
}
