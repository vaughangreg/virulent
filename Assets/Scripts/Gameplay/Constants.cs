using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All constants.
/// </summary>
public static class Constants
{
	/// <summary>
	/// Used to check if object recieving damage will take damage from attacker
	/// </summary>
	[System.Flags]
	public enum CombatFlags 
	{
		None          = 0x00000000,
		Capsid        = 0x00000002,
		Antibody      = 0x00000004,
		ProteinM      = 0x00000008,
		ProteinLP     = 0x00000010,
		ProteinN      = 0x00000020,
		Genome        = 0x00000040,
		Monomer       = 0x00000080,
		GenomeArmored = 0x00000001,	// Taking up the old slot.
		MRna          = 0x00000100,
		Ribosome      = 0x00000200,
		Slicer        = 0x00000400,
		Proteasome    = 0x00000800,
		ProteinG      = 0x00001000,
		PRR		      = 0x00002000,
		Goal          = 0x00004000,
		CellmRNA      = 0x00008000,
		Cytoskeleton  = 0x00010000,
		Caspace       = 0x00020000,
		DNA			  = 0x00040000,
		Vesicle       = 0x00080000,
		BuddingSite   = 0x00100000,
		
		//Groups
		Virus      		   = 0x0000003A,
		Proteins   		   = 0x00001038,
		GenomesAll         = (0x00000001 | 0x00000040),
		Genomic    		   = 0x00000140,
		Cell       		   = 0x00008E84,
		MProteinOffense    = 0x00002800,
		RibosomeGoal       = 0x00004200,
		SlicerProteasome   = (0x00000400 | 0x00000800),
		ArmoredAndProteins = (0x00000001 | 0x00001038),
		ProteasomeAndPRR   = (0x00000800 | 0x00002000)
	}
	
	/// <summary>
	/// Used for command costs.
	/// </summary>
	public enum Energy 
	{
		Atp = 0,
		ProteinG,
		ProteinM,
		ProteinN,
		ProteinLP,
		VesicleG,
		Genome,
		Capsid,
		Count
	}
	
	//Level Names
	public static List<string> LEVEL_NAMES = new List<string>() {
		"'We Arrive'",
		"'We Encapsulate'",
		"'We Alert'",
		"'We Evade'",
		"'We Infect'",
		"'We Steal'",
		"'We Transcribe'",
		"'We Defend'",
		"'We Fight'",
		"'We Disrupt'",
		"'We Accelerate'",
		"'We Unleash'",
		"'We Prepare'",
		"'We Escape'",
		"'We Repeat'"
	};
	
	//Level Numbers
	public static List<string> LEVELS = new List<string>() {
		"Level 0",
		"Level 1",
		"Level 3",
		"Level 4",
		"Level 5",
		"Level 6",
		"Level 7",
		"Level 8",
		"Level 9",
		"Level 10",
		"Level 11",
		"Level 12",
		"Level 13",
		"Level 14",
		"Level 15"
	};
	
	//Challenge Level Names
	public static List<string> CHALLENGE_LEVELS = new List<string>() {
		"Antibody Challenge",
		"Slicer Challenge",
		"Proteasome Challenge",
		"BCell Challenge",
		"Distance Challenge",
		"Receptor Challenge",
		"PRR Challenge"
	};
	
	//Tags
	public const string TAG_CAPSID		= "Capsid";
	public const string TAG_MRNAG		= "G mRNA";
	public const string TAG_MRNAM		= "M mRNA";
	public const string TAG_MRNAN		= "N mRNA";
	public const string TAG_MRNALP		= "LP mRNA";
	public const string TAG_ANTIGENOME	= "Antigenome";
	public const string TAG_GENOME		= "Genome";
	public const string TAG_PROTEING	= "G Protein";
	public const string TAG_PROTEINM	= "M Protein";
	public const string TAG_PROTEINN	= "N Protein";
	public const string TAG_PROTEINLP	= "LP Proteins";
	public const string TAG_DEADCAPSID  = "DeadCapsid";
	public const string TAG_ARMORANTIGENOME = "Armored Antigenome";
	public const string TAG_ARMORGENOME = "Armored Genome";
	public const string TAG_PROTEINRECEPTOR = "G Protein Receptor";
	public const string TAG_BUDDINGSITE = "Budding Site";
	public const string TAG_RIBOSOME = "Ribosome";
	
	public const string TAG_ANTIBODY	= "Antibody";
	public const string TAG_MONOMER		= "Monomer";
	public const string TAG_SLICER		= "Slicer Enzyme";
	public const string TAG_PROTEASOME 	= "Proteasome";
	public const string TAG_PRR 		= "PRR";
	public const string TAG_GOLGI		= "Golgi Apparatus";
	public const string TAG_PROTEASOMEMRNA = "Proteasome mRNA";
	public const string TAG_PRRMRNA     = "PRR mRNA";
	public const string TAG_SLICERMRNA  = "Slicer mRNA";
	public const string TAG_NUCLEUS 	= "Nucleus";
	public const string TAG_AITarget 	= "AITarget";
	public const string TAG_NUCLEARPORE = "Nuclear Pore";
	public const string TAG_CLOGGEDPORE = "Clogged Pore";
	public const string TAG_DNA 		= "DNA";
	public const string TAG_CASPACE     = "Executioner";
	public const string TAG_CASPACE_MRNA= "Executioner mRNA";
	
	public const string TAG_SELECTIONRING = "SelectionRing";
	public const string TAG_BEACON = "Beacon";
	
	public const string TAG_CYTOSKELETON      = "Cytoskeleton";
	public const string TAG_CYTO_DEAD      	  = "DeadCytoskeleton";
	
	public const string TAG_VESICLE = "Vesicle";
	
	public const string COLLECTION_VIRUS  = "VirusTags";
	public const string COLLECTION_CELL   = "CellTags";
	public const string COLLECTION_GENOME = "GenomeTags";
	public const string COLLECTION_GENOME_CAPSID = "GenomeCapsidTags";
	public const string COLLECTION_NUCLEARPORE = "NuclearPoreTags";
	public const string COLLECTION_ENVIRONMENT = "EnvironmentTags";
	public const string COLLECTION_PRODUCERS = "ProducerTags";
	
	/// <summary>
	/// List of all TAG constants set
	/// </summary>
	public static List<string> TAGS_LIST = new List<string>() {
		Constants.TAG_AITarget,
		Constants.TAG_ANTIBODY,
		Constants.TAG_ANTIGENOME,
		Constants.TAG_ARMORGENOME,
		Constants.TAG_BEACON,
		Constants.TAG_CAPSID,
		Constants.TAG_CLOGGEDPORE,
		Constants.TAG_DEADCAPSID,
		Constants.TAG_GENOME,
		Constants.TAG_GOLGI,
		Constants.TAG_MONOMER,
		Constants.TAG_MRNAG,
		Constants.TAG_MRNALP,
		Constants.TAG_MRNAM,
		Constants.TAG_MRNAN,
		Constants.TAG_NUCLEARPORE,
		Constants.TAG_NUCLEUS,
		Constants.TAG_PROTEASOME,
		Constants.TAG_PROTEASOMEMRNA,
		Constants.TAG_PROTEING,
		Constants.TAG_PROTEINLP,
		Constants.TAG_PROTEINM,
		Constants.TAG_PROTEINN,
		Constants.TAG_PRR,
		Constants.TAG_PRRMRNA,
		Constants.TAG_SELECTIONRING,
		Constants.TAG_SLICER,
		Constants.TAG_SLICERMRNA,
		Constants.TAG_CYTOSKELETON,
		Constants.TAG_CASPACE,
		Constants.TAG_CASPACE_MRNA,
		Constants.TAG_CYTO_DEAD,
		Constants.TAG_VESICLE,
	};
	
	//dictionary of collection names(key) and lists of tags to be counted in that collection(value)
	public static Dictionary<string, List<string>> TAG_COLLECTIONS = new Dictionary<string, List<string>>() {
		{COLLECTION_VIRUS, new List<string>() {
				Constants.TAG_CAPSID,
				Constants.TAG_MRNAG,
				Constants.TAG_MRNAM,
				Constants.TAG_MRNAN,
				Constants.TAG_MRNALP,
				Constants.TAG_ANTIGENOME,
				Constants.TAG_GENOME,
				Constants.TAG_ARMORGENOME,
				Constants.TAG_PROTEING,
				Constants.TAG_PROTEINM,
				Constants.TAG_PROTEINN,
				Constants.TAG_PROTEINLP,
				Constants.TAG_DEADCAPSID, // <<< HACK: this may not be the best way to do combat death counting...?
				Constants.TAG_VESICLE,
			}
		},
		{COLLECTION_CELL, new List<string>() {
				Constants.TAG_ANTIBODY,
				Constants.TAG_SLICER,
				Constants.TAG_PROTEASOME,
				Constants.TAG_MONOMER,
				Constants.TAG_PRR,
				Constants.TAG_PRRMRNA,
				Constants.TAG_SLICERMRNA,
				Constants.TAG_PROTEASOMEMRNA,
				Constants.TAG_CASPACE,
				Constants.TAG_CASPACE_MRNA
			}
			
		},
		{COLLECTION_GENOME, new List<string>() {
				Constants.TAG_GENOME,
				Constants.TAG_ARMORGENOME
			}
		},
		{COLLECTION_GENOME_CAPSID, new List<string>() {
				Constants.TAG_GENOME,
				Constants.TAG_ARMORGENOME,
				Constants.TAG_CAPSID
			}
		},
		{COLLECTION_NUCLEARPORE, new List<string>() {
				Constants.TAG_CLOGGEDPORE,
				Constants.TAG_NUCLEARPORE
			}
		},
		{COLLECTION_ENVIRONMENT, new List<string>() {
				Constants.TAG_CYTOSKELETON,
				Constants.TAG_RIBOSOME,
				Constants.TAG_CYTO_DEAD,
			}
		},
		{COLLECTION_PRODUCERS, new List<string>() {
				Constants.TAG_GENOME,
				Constants.TAG_RIBOSOME,
				Constants.TAG_GOLGI,
				Constants.TAG_BUDDINGSITE,
				Constants.TAG_ARMORGENOME,
				Constants.TAG_ANTIGENOME,
				Constants.TAG_ARMORANTIGENOME 
			}
		}
	};
	
	public const int MIN_LEVEL_FOR_CHALLENGES = 4;
	public const float TIME_PAUSE_THRESHOLD = 0.00000000000001f;
	public const float TIME_MIN             = 0.0000000000000001f;
}
