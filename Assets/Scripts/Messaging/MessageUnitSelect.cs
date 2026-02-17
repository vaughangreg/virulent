using UnityEngine;

/// <summary>
/// Sent on unit selection.
/// </summary>
public class MessageUnitSelect: Message {
	/// <summary>
	/// Records that the unit was selected.
	/// </summary>
	/// <param name="aSource">
	/// A <see cref="GameObject"/>
	/// </param>
	
	public PopupManager.PresetIDs popupId = PopupManager.PresetIDs.EMPTY; //Start empty
	
	public MessageUnitSelect(GameObject aSource) : base(aSource, Dispatcher.UNIT_SELECT) {
		base.Send();
	}
	public MessageUnitSelect(GameObject aSource, PopupManager.PresetIDs aPopupId) : base(aSource, Dispatcher.UNIT_SELECT) {
		popupId = aPopupId;
		base.Send();
	}
}
