using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Displays a circular button GUI with specified commands.
/// </summary>
//[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnergyContainer))]
[RequireComponent(typeof(Producer))]
public class CircularMenu : MonoBehaviour {
	public Command[] commands;
	public float distanceInPixels = 100.0f;
	public Constants.Energy displayedEnergy = Constants.Energy.Atp;
	public Producer producer;
	public bool childUsesFixedRotation = true;
	
	protected List<GameObject> m_buttons = new List<GameObject>();
	protected Rect m_tooltipRect;
	
	protected GameObject m_energybar;
	
	#region Initializers
	/// <summary>
	/// Determine button & text positions.
	/// </summary>
	void Start() {
		useGUILayout = false;
		//float initialDegreeOffset = 180.0f - (45.0f * commands.Length); // 90.0f - (11.25f * Mathf.Max(0, commands.Length - 1));
		//float angleSpacing = 360.0f / commands.Length;
		
		//float initialDegreeOffset = 90.0f;
		//float angleSpacing = 45.0f;
		
		float initialDegreeOffset;
		float angleSpacing;
		
		if (commands.Length < 4) {
			angleSpacing = 180.0f / (commands.Length + 1);
			initialDegreeOffset = angleSpacing;
		}
		else if (commands.Length == 4) {
			angleSpacing = 60.0f;
			initialDegreeOffset = 0.0f;
		}
		else {
			angleSpacing = 360.0f / commands.Length;
			switch (commands.Length) {
				case 5:
					initialDegreeOffset = -54.0f;
					break;
				case 7:
					initialDegreeOffset = -12.0f;
					break;
				default:
					initialDegreeOffset = (-0.5f * (commands.Length - 4));
					break;
			}
		}
		
		float yPos = Camera.main.transform.position.y - Camera.main.nearClipPlane - 2.5f;
		
		InitializeEnergyBar();
		
		GuiResources gui = GuiResources.instance;
		for (int i = 0; i < commands.Length; ++i) {
			float angle = Mathf.Deg2Rad * (i * angleSpacing + initialDegreeOffset);
			Vector3 boxCenter = new Vector3(Mathf.Cos(angle) * distanceInPixels,
			                                yPos,
			                              	Mathf.Sin(angle) * distanceInPixels);
			
			GameObject aButton = gui.ButtonForUnit(commands[i].unitProduced);
			aButton.transform.parent = m_energybar.transform;
			aButton.transform.position = transform.position + boxCenter;
			
			Vector3 targetPosition = m_energybar.transform.position;
			aButton.transform.LookAt(targetPosition);
			
			Vector3 newRotation = aButton.transform.rotation.eulerAngles;
			newRotation.x = 0.0f;
			newRotation.y += 180.0f;
			aButton.transform.rotation = Quaternion.Euler(newRotation);
			
			//aButton.SendMessage("OnStay", SendMessageOptions.RequireReceiver);
			aButton.active = false;
			m_buttons.Add(aButton);
		}
		
		m_tooltipRect = new Rect(-distanceInPixels * 0.5f, 0,
			distanceInPixels, distanceInPixels * 0.5f);
		
		if (producer == null) Debug.LogError("CircularMenu: producer not set", gameObject);
	}
	
	/// <summary>
	/// Sets up links to the energy bar.
	/// </summary>
	void InitializeEnergyBar() {
		Transform childTransform = transform.Find(GuiResources.NAME_ENERGY_BAR);
		if (!childTransform) Debug.LogError("CircularMenu: No child named " + GuiResources.NAME_ENERGY_BAR);
		m_energybar = childTransform.gameObject;
		m_energybar.SendMessage("OnStay", SendMessageOptions.DontRequireReceiver);
		m_energybar.SetActiveRecursively(false);
		childTransform.rotation = childUsesFixedRotation ? Quaternion.identity : Quaternion.Euler(0.0f, 180.0f, 0.0f);
		
		Vector3 newPosition = childTransform.position;
		newPosition.y = Camera.main.transform.position.y - Camera.main.nearClipPlane - 0.5f;
		childTransform.position = newPosition;
	}
	#endregion
	
	#region Event/Message handling	
	/// <summary>
	/// Disable the menu.
	/// </summary>
	/// <see cref="OnDisable"/>
	void OnDeselect(InputManager source) {
		if (this.enabled && source.transform.IsChildOf(m_energybar.transform)) {
			return;
		}
		
		Selectable selector = GetComponent<Selectable>();
		selector.isSelected = false;
		
		HideChildren();
	}
	
	/// <summary>
	/// Hides GUI.
	/// </summary>
	void HideChildren() {
		// Send message here?
		if (!this.enabled) return;
		foreach (GameObject go in m_buttons) {
			go.SendMessage("OnDeselect", SendMessageOptions.DontRequireReceiver);
		}
		m_energybar.SetActiveRecursively(false);
	}
	
	/// <summary>
	/// Displays the GUI.
	/// </summary>
	void ShowChildren() {
		// Send message here?
		m_energybar.SetActiveRecursively(true);
	}
	
	/// <summary>
	/// Shows or hides the menu as needed.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnTap(InputManager source) {
		Selectable selector = GetComponent<Selectable>();
		
		if (selector.isSelected) {	// We are already selected.
			source.ClearSelectionIfNeeded();
			HideChildren();
		}
		else {						// We are not.
			source.Select(gameObject);
			selector.isSelected = true;
			if (enabled) ShowChildren();
		}
	}
	
	/// <summary>
	/// Hide children.
	/// </summary>
	/// <param name="source">
	/// A <see cref="InputManager"/>
	/// </param>
	void OnSlideStart(InputManager source) {
		HideChildren();	
	}
	#endregion
	
	#region Updates
	/// <summary>
	/// Draws the menu.
	/// </summary>
	void OnGUI() {
		GuiResources guiResource = GuiResources.instance;
		Vector3 screenPosition = guiResource.GetScreenPosition(transform.position, distanceInPixels);
		
		// NOTE: tooltips won't work on iPad, I think…
		Rect tooltipRect = guiResource.CenterAtGuiPoint(m_tooltipRect, screenPosition);
		GUI.Label(tooltipRect, GUI.tooltip, guiResource.commandTooltipStyle);
	}
	
	/// <summary>
	/// Updates the buttons & commands.
	/// </summary>
	void Update() {
		EnergyContainer battery = GetComponent<EnergyContainer>();
		for (int i = 0; i < commands.Length; ++i) {
			Command aCommand = commands[i];
			Button aButton = m_buttons[i].GetComponent<Button>();
			
			aButton.isEnabled = (producer.upgradeLevel >= aCommand.minUpgradeRequired
			                     && !aCommand.isProducing
								 && battery.IsEnergySufficient(aCommand.costs));
			aButton.countdown = aCommand.productionRatio;
			if (aButton.isPressed) {
				battery.ConsumeEnergy(aCommand.costs);
				aCommand.StartProduction();
				new MessageBuildCommandStarted(gameObject, aCommand.displayName, aCommand.timeRequired);
				aButton.isCoolingDown = true;
			}
			else if (aCommand.IsProductionReady(Time.deltaTime * producer.productionSpeedPercent)) {
				if (!producer.Produce(aCommand.unitProduced)) {
					battery.AddEnergy(aCommand.costs);	
				}
				aButton.isCoolingDown = false;
			}
		}
		UpdateEnergyDisplay(battery);
	}
	
	void UpdateEnergyDisplay(EnergyContainer aBattery) {
		float targetAlpha = 1.0f - aBattery.RatioFor(displayedEnergy);
		
		if (Mathf.Approximately(targetAlpha, 1.0f)) {
			m_energybar.GetComponent<Renderer>().enabled = false;
		}
		else {
			// NOTE: We need this because the alpha at 0.8f, though it seems correct
			// in the image, renders incorrectly. At 0.3f, the art may be incorrect,
			// but this change accounts for it.
			if (Mathf.Approximately(targetAlpha, 0.8f)) targetAlpha = 0.85f;
			else if (Mathf.Approximately(targetAlpha, 0.3f)) targetAlpha = 0.26f;
			
			m_energybar.GetComponent<Renderer>().enabled = true;
			m_energybar.GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.Max(targetAlpha, 0.01f));
		}
	}
	#endregion
	
	#region Utility methods
	public bool IsAnyCommandEnabled() {
		bool isEnabled = false;
		
		EnergyContainer battery = GetComponent<EnergyContainer>();
		foreach (Command aCommand in commands) {
			isEnabled = (producer.upgradeLevel >= aCommand.minUpgradeRequired
						 && battery.IsEnergySufficient(aCommand.costs)
	                     && !aCommand.isProducing);
			if (isEnabled) break;
		}
		
		return isEnabled;
	}
	
	#endregion
}
