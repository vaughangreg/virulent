using UnityEngine;
using System.Collections;

public class MessageFeedbackDisplay : Message {

	public MessageFeedbackDisplay(GameObject aSource) : base(aSource, Dispatcher.FEEDBACK_DISPLAY) {
		base.Send();
	}

}
