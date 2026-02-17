using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// THE data class. Every chunk of logged data must inherit from this in some way.
/// If you are adding more fields to a child class of ADAGEData, you should also implement the
/// InitFromJSON method so that your object can be recreated from JSON.
/// </summary>

[ADAGE.BaseClass]
public class ADAGEData
{
	public static ADAGEData Copy(ADAGEData data)
	{
		Type type = data.GetType();
		ADAGEData newData = (ADAGEData) Activator.CreateInstance(type);

		foreach (PropertyInfo sourcePropertyInfo in type.GetProperties())  
		{
			PropertyInfo destPropertyInfo = type.GetProperty(sourcePropertyInfo.Name);
			
			destPropertyInfo.SetValue(
				newData,
				sourcePropertyInfo.GetValue(data, null),
				null);
		}

		foreach (FieldInfo sourceFieldInfo in type.GetFields())  
		{
			FieldInfo destFieldInfo = type.GetField(sourceFieldInfo.Name);

			destFieldInfo.SetValue(
				newData,
				sourceFieldInfo.GetValue(data));
		}

		return newData;
	}


	public string application_name {get; set;}
	public string application_version {get; set;}
	public string adage_version = ADAGE.VERSION;
	public string timestamp { get; set; }
	public string session_token { get; set;}
	public string game_id { get; set;}
	public List<string> ada_base_types { get; set;}
	public string key { get; set; } 

	public static ADAGEData CreateFromJSON(string json)
	{	
		ADAGEData baseData = LitJson.JsonMapper.ToObject<ADAGEData>(json);
		
		if(baseData.key != null && baseData.key != "")
		{
			Type theType = ReflectionUtils.FindType(baseData.key);
			if(theType != null)
			{
				//ADAGEData output = Activator.CreateInstance(theType) as ADAGEData;
				//output.Copy(baseData);
				ADAGEData output = ADAGEData.Copy(baseData);

				output.InitFromJSON(json);
				return output;
			}
		}	
		
		return null;			
	}

	public ADAGEData()
	{
		this.key = GetType().ToString();
	}

	//TODO: Can we reflect here?
	public virtual void InitFromJSON(string input){}
}

//public class ADAGEStartGame : ADAGEPlayerEvent {}
[ADAGE.BaseClass]
public class ADAGEStartGame : ADAGEData 
{
	public ADAGEGameInformation gameInformation;
}

[ADAGE.BaseClass]
public class ADAGEQuitGame : ADAGEPlayerEvent {}

[ADAGE.BaseClass]
public class ADAGEStartSession : ADAGEData
{
	public ADAGEDeviceInfo deviceInfo;
	
	public ADAGEStartSession()
	{
		deviceInfo = new ADAGEDeviceInfo();
	}
}

[ADAGE.BaseClass]
public class ADAGEEndSession : ADAGEData{}

[ADAGE.BaseClass]
public class ADAGEErrorEvent : ADAGEPlayerEvent
{
	public string message;

	public ADAGEErrorEvent()
	{
		this.message = "";
	}

	public ADAGEErrorEvent(string message)
	{
		this.message = message;
	}
}

public class ADAGEDeviceInfo
{
	public string device_model = ""; //The model of the device (Read Only).
	public string device_type = ""; //Returns the kind of device the application is running on.
	public string device_unique_identifier = ""; //A unique device identifier. It is guaranteed to be unique for every device (Read Only).

	public int graphics_device_id = -1; //The identifier code of the graphics device (Read Only).
	public string graphics_device_name = ""; //The name of the graphics device (Read Only).
	public string graphics_device_vendor = ""; //The vendor of the graphics device (Read Only).
	public int graphics_device_vendor_id = -1; //The identifier code of the graphics device vendor (Read Only).
	public string graphics_device_version = ""; //The graphics API version supported by the graphics device (Read Only).
	public int graphics_memory_size = 0; //Amount of video memory present (Read Only).
	public int graphics_pixel_fillrate = -1; //Approximate pixel fill-rate of the graphics device (Read Only).
	public int graphics_shader_level = -1; //Graphics device shader capability level (Read Only).

	public string operating_system = ""; //Operating system name with version (Read Only).
	public int processor_count = -1; //Number of processors present (Read Only).
	public string processor_type = ""; //Processor name (Read Only).
	public int system_memory_size = -1; //Amount of system memory present (Read Only).

	public bool supports_accelerometer = false; //Is an accelerometer available on the device?
	public bool supports_compute_shaders = false; //Are compute shaders supported? (Read Only)
	public bool supports_image_effects = false; //Are image effects supported? (Read Only)
	public bool supports_instancing = false; //Is GPU draw call instancing supported? (Read Only)
	public bool supports_location_service = false; //Is the device capable of reporting its location?
	public bool supports_render_textures = false; //Are render textures supported? (Read Only)
	public bool supports_shadows = false; //Are built-in shadows supported? (Read Only)
	public int supports_stencil = -1; //Is the stencil buffer supported? (Read Only)
	public bool supports_vibration = false; //Is the device capable of providing the user haptic feedback by vibration?

	public ADAGEDeviceInfo()
	{
		this.device_model = SystemInfo.deviceModel;
		this.device_type = SystemInfo.deviceType.ToString();
		this.device_unique_identifier = SystemInfo.deviceUniqueIdentifier;

		this.graphics_device_id = SystemInfo.graphicsDeviceID;
		this.graphics_device_name = SystemInfo.graphicsDeviceName;
		this.graphics_device_vendor = SystemInfo.graphicsDeviceVendor;
		this.graphics_device_vendor_id = SystemInfo.graphicsDeviceVendorID;
		this.graphics_device_version = SystemInfo.graphicsDeviceVersion;
		this.graphics_memory_size = SystemInfo.graphicsMemorySize;
		this.graphics_pixel_fillrate = SystemInfo.graphicsPixelFillrate;
		this.graphics_shader_level = SystemInfo.graphicsShaderLevel;

		this.operating_system = SystemInfo.operatingSystem;
		this.processor_count = SystemInfo.processorCount;
		this.processor_type = SystemInfo.processorType;
		this.system_memory_size = SystemInfo.systemMemorySize;

		this.supports_accelerometer = SystemInfo.supportsAccelerometer;
		this.supports_compute_shaders = SystemInfo.supportsComputeShaders;
		this.supports_image_effects = SystemInfo.supportsImageEffects;
		this.supports_instancing = SystemInfo.supportsInstancing;
		this.supports_location_service = SystemInfo.supportsLocationService;
		this.supports_render_textures = SystemInfo.supportsRenderTextures;
		this.supports_shadows = SystemInfo.supportsShadows;
		this.supports_stencil = SystemInfo.supportsStencil;
		this.supports_vibration = SystemInfo.supportsVibration;
	}
}

[ADAGE.BaseClass]
public class ADAGEScreenshot : ADAGEData
{
	public string cameraName;	
	public byte[] shot;
	
	public ADAGEScreenshot()
	{
		cameraName = "";	
	}
	
	public ADAGEScreenshot(string source)
	{
		cameraName = source;	
	}
}

[ADAGE.BaseClass]
public class ADAGEKeyboardEvent : ADAGEData
{
	public List<int> ASCII;

	public ADAGEKeyboardEvent()
	{
		ASCII = new List<int>();
	}

	public ADAGEKeyboardEvent(int[] codes)
	{
		ASCII = new List<int>(codes);
	}

	public void AddCode(int code)
	{
		ASCII.Add(code);
	}
}

[ADAGE.BaseClass]
public class ADAGEMouseEvent : ADAGEData
{
	public Vector3 position;
	public string button;

	public ADAGEMouseEvent()
	{
		position = Vector3.zero;
		button = "";
	}

	public ADAGEMouseEvent(Vector3 position, string button)
	{
		this.position = position;
		this.button = button;
	}
}

public class ADAGEVirtualContext
{
	public string level;  //The name of the level(map, scene, stage, etc)
	public List<string> active_units; //Names of all the currently active game units. This can be used as a flat list of tags for processing actions within the scope of the units
	
	public ADAGEVirtualContext()
	{
		active_units = new List<string>();
		level = "";
	}
	
	public ADAGEVirtualContext(string curLevel)
	{
		active_units = new List<string>();
		level = curLevel;
	}
	
	public void Add(string id)
	{
		if(IsTracking(id))
		{
			throw new ADAGEStartContextException(id);	
		}
				
		active_units.Add(id);
	}
	
	public void Remove(string id)
	{
		if(!IsTracking(id))
		{
			throw new ADAGEEndContextException(id);	
		}
			
		active_units.Remove(id);
	}
	
	public bool IsTracking(string id)
	{
		if(active_units == null)
			active_units = new List<string>();
		
		return active_units.Contains(id);
	}

	public void Clear()
	{
		//This will clear the active units list. Be careful with calling this.
		active_units.Clear();
	}
}

public class ADAGEPositionalContext
{
	public float x;
	public float y;
	public float z;
	//Euler angles for rotation
	public float rotx;
	public float roty;
	public float rotz;
	
	public ADAGEPositionalContext()
	{
		x = 0f;
		y = 0f;
		z = 0f;
		rotx = 0f;
		roty = 0f;
		rotz = 0f;
	}
	
	public void setPosition(float iX, float iY, float iZ)
	{
		x = iX;
		y = iY;
		z = iZ;
	} 

	public void setRotation(float iX, float iY, float iZ)
	{
		rotx = iX;
		roty = iY;
		rotz = iZ;
	}
}

[ADAGE.BaseClass]
public class ADAGEContext : ADAGEData
{
	public string name; //Should be unique
	public string parent_name; //This can be left blank if there is no parent for this unit
	public bool success;
	
	public ADAGEContext()
	{
		name = "";
		parent_name = "";
		success = false;
	}
}

[ADAGE.BaseClass]
public class ADAGEContextStart  : ADAGEContext 
{
	public ADAGEContextStart() : base()
	{}
}

[ADAGE.BaseClass]
public class ADAGEContextEnd : ADAGEContext 
{
	public ADAGEContextEnd() : base()
	{}
}

[ADAGE.BaseClass]
public abstract class ADAGEEventData : ADAGEData
{
	public abstract void Update();
}

/// <summary>
/// Used for logging player events in the world. This is different than normal ADAGEData logging in that this event
/// is capable of updating it's virtual and positional context fields before being logged.
/// </summary>
[ADAGE.BaseClass]
public class ADAGEPlayerEvent : ADAGEGameEvent
{
	public ADAGEPositionalContext positional_context;
	
	public ADAGEPlayerEvent() : base()
	{
		//positional_context = new ADAGEPositionalContext();
	}
	
	public override void Update()
	{
		base.Update();
		//positional_context = ADAGE.PositionalContext;
	}	
}

/// <summary>
/// Used for logging game events. This is different than normal ADAGEData logging in that this event
/// is capable of updating it's virtual context field before being logged.
/// </summary>
[ADAGE.BaseClass]
public class ADAGEGameEvent : ADAGEEventData
{
	public ADAGEVirtualContext virtual_context;
	
	public ADAGEGameEvent()
	{
		//virtual_context = new ADAGEVirtualContext();	
	}
	
	public override void Update()
	{
		//virtual_context = ADAGE.VirtualContext;		
	}
}

public class ADAGEDefaultOverride
{
	public string url;
	public string playername;
	public string password;
}