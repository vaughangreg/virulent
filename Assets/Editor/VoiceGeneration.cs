using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;

/// <summary>
/// Generates WAVE files from an XML source.
/// </summary>
class VoiceGeneration : MonoBehaviour {
	const string COMMAND_NAME = "/usr/bin/say";
	// /Assets/Audio/Sound Effects/Generated/Genome/Genome-Path1
	const string COMMAND_OUTPUT_PATH = "\"{0}/Audio/Sound Effects/Generated/{1}/{1}-{2}{3}";
	const string COMMAND_FORMAT = "-v {0} -o {1}.aiff\" {2}";
	
	const string CONVERSION_NAME = "/usr/bin/afconvert";
	const string CONVERSION_ARGS = "-f WAVE -d LEI16 {0}.aiff\" {0}.wav\"";
	
	/// <summary>
	/// Validates whether or not we can use the command.
	/// </summary>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	[MenuItem("Virulent/Audio/Generate Voices", true)]
	static bool ValidateGenerateVoices() {
		if (Application.platform != RuntimePlatform.OSXEditor) return false;
		
		foreach (Object anObject in Selection.objects) {
			if (anObject.GetType() == typeof(TextAsset)) return true;
		}
		return false;
	}
	
	/// <summary>
	/// Starts processing each object.
	/// </summary>
	/// <param name="aCommand">
	/// A <see cref="MenuCommand"/>
	/// </param>
	[MenuItem("Virulent/Audio/Generate Voices")]
	static void GenerateVoices(MenuCommand aCommand) {
		Object[] activeObjects = Selection.GetFiltered(typeof(TextAsset),
		                                               SelectionMode.Assets);
		foreach (Object anObject in activeObjects) {
			ProcessObject((TextAsset)anObject);
		}
	}
	
	/// <summary>
	/// Parses each XML file.
	/// </summary>
	/// <param name="anObject">
	/// A <see cref="TextAsset"/>
	/// </param>
	static void ProcessObject(TextAsset anObject) {
		StringReader sourceString = new StringReader(anObject.text);
		
		string currentUnit = "NullUnit";
		string currentType = "NullType";
		string currentVoice = "Alex";
		int currentNumber = 0;
		
		using (XmlReader reader = XmlReader.Create(sourceString)) {
			while (reader.Read()) {
				if (reader.IsStartElement()) {
					switch (reader.Name) {
						case "unit":
							currentUnit = reader["name"];
							if (currentUnit == null) {
								Debug.LogError("VoiceGeneration: No 'name' tag found in <unit>", anObject);
								return;
							}
							currentVoice = (reader["voice"] != null) ? reader["voice"] : "Alex";
							break;
						case "statement":
							currentType = reader["type"];
							if (currentType == null) {
								Debug.LogError("VoiceGeneration: No 'type' in voice for " + currentUnit, anObject);
								return;
							}
							currentNumber = 0;
							break;
						case "say":
							string message = reader["value"];
							if (message == null) {
								Debug.LogError("VoiceGeneration: No 'value' in " + currentUnit + "/" + currentType, anObject);
								return;
							}
							message = message.Replace("'", "\\'");
						
							Speak(currentUnit, currentType, currentVoice, currentNumber++, message);
							break;
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Saves an AIFF file, converts it to WAVE, and removes the AIFF.
	/// </summary>
	/// <param name="aUnit">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="aType">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="aVoice">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="aNumber">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="aMessage">
	/// A <see cref="System.String"/>
	/// </param>
	static void Speak(string aUnit, string aType, string aVoice, int aNumber, string aMessage) {
		// Create the AIFF
		System.Diagnostics.Process theProcess = new System.Diagnostics.Process();
		theProcess.StartInfo.FileName = COMMAND_NAME;
		
		string outputPath = System.String.Format(COMMAND_OUTPUT_PATH, Application.dataPath,
		                                                      aUnit, aType, aNumber);
		// Ignore the first "
		string directory = Path.GetDirectoryName(outputPath).Substring(1);
		if (!Directory.Exists(directory)) {
			Debug.Log("VoiceGeneration: Creating " + directory);
			Directory.CreateDirectory(directory);
		}
		
		theProcess.StartInfo.Arguments = System.String.Format(COMMAND_FORMAT, aVoice, 
		                                                      outputPath,
		                                                      aMessage);
		theProcess.StartInfo.RedirectStandardError = true;
		theProcess.StartInfo.RedirectStandardOutput = true;
		theProcess.StartInfo.CreateNoWindow = true;
		theProcess.StartInfo.UseShellExecute = false;
		
		theProcess.Start();
		string error = theProcess.StandardError.ReadToEnd();
		theProcess.WaitForExit();
		if (theProcess.ExitCode != 0) {
			Debug.Log(aUnit + "/" + aType + "/ Creation: " + theProcess.ExitCode + ": " +  error);
			return;
		}
		
		// Convert to WAVE
		theProcess = new System.Diagnostics.Process();
		theProcess.StartInfo.FileName = CONVERSION_NAME;
		theProcess.StartInfo.Arguments = System.String.Format(CONVERSION_ARGS, outputPath);
		
		theProcess.StartInfo.RedirectStandardError = true;
		theProcess.StartInfo.RedirectStandardOutput = true;
		theProcess.StartInfo.CreateNoWindow = true;
		theProcess.StartInfo.UseShellExecute = false;
		
		theProcess.Start();
		error = theProcess.StandardError.ReadToEnd();
		theProcess.WaitForExit();
		if (theProcess.ExitCode != 0) {
			Debug.Log("Conversion: " + theProcess.ExitCode + ": " +  error);
			return;
		}
		
		// Remove the original file
		// Note that we need to remove quotes here.
		File.Delete(outputPath.Substring(1) + ".aiff");
	}
}
