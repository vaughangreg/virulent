using UnityEngine;

/// <summary>
/// Sent by PostRenderNotifier on post rendering.
/// </summary>
public class MessagePostRender : Message {
	
	/// <summary>
	/// Send the on post render message.
	/// </summary>
	public MessagePostRender() : base(null, Dispatcher.RENDER_POST) {
		base.Send();
	}
}
