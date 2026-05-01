using System;
using System.ArrayExtensions;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AOT;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using Steamworks;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyVersion("0.0.0.0")]
public class LocalizedTextObject : MonoBehaviour
{
	public string TheKey;

	public string TheTag = "";

	public string TheValue = "";

	private TextMeshProUGUI MyTextMeshPro;

	private TMP_Dropdown ForEffectsDropDown;

	private void Start()
	{
		if (base.gameObject.name == "EffectsDropdown")
		{
			ForEffectsDropDown = GetComponent<TMP_Dropdown>();
			ForEffectsDropDown.options[0].text = LocalizerManager.GetTranslatedValue("HighEffectsText");
			ForEffectsDropDown.options[1].text = LocalizerManager.GetTranslatedValue("LowEffectsText");
			ForEffectsDropDown.options[2].text = LocalizerManager.GetTranslatedValue("NoEffectsText");
		}
		else
		{
			MyTextMeshPro = GetComponent<TextMeshProUGUI>();
			string text = "";
			text = ((!(TheTag == "")) ? LocalizerManager.GetTranslatedThenReplaceValues(TheKey, TheTag, TheValue) : LocalizerManager.GetTranslatedValue(TheKey));
			MyTextMeshPro.text = text;
		}
	}

	public void UpdateFromOutside()
	{
		Start();
	}
}
public class LocalizerManager : MonoBehaviour
{
	public delegate void LanguageChange();

	public static Dictionary<string, LocalizedInformation> AllTextData = new Dictionary<string, LocalizedInformation>();

	private LocalizationDataList LocData = new LocalizationDataList();

	public TextAsset JsonDataText;

	public TreeInfo MainTree;

	public static event LanguageChange OnLanguageChange;

	public void AwakeMe()
	{
		if (AllTextData.Count == 0)
		{
			LocData = JsonUtility.FromJson<LocalizationDataList>(JsonDataText.text);
			for (int i = 0; i < LocData.LDataForJson.Count; i++)
			{
				string english_Value = LocData.LDataForJson[i].English_Value;
				string fRENCH_Value = LocData.LDataForJson[i].FRENCH_Value;
				string pORTUGUESE_Value = LocData.LDataForJson[i].PORTUGUESE_Value;
				string gERMAN_Value = LocData.LDataForJson[i].GERMAN_Value;
				string cHINESE_Value = LocData.LDataForJson[i].CHINESE_Value;
				string rUSSIAN_Value = LocData.LDataForJson[i].RUSSIAN_Value;
				string jAPANESE_Value = LocData.LDataForJson[i].JAPANESE_Value;
				string sPANISH_Value = LocData.LDataForJson[i].SPANISH_Value;
				string iTALIAN_Value = LocData.LDataForJson[i].ITALIAN_Value;
				string kOREAN_Value = LocData.LDataForJson[i].KOREAN_Value;
				string cZECH_Value = LocData.LDataForJson[i].CZECH_Value;
				string pOLISH_Value = LocData.LDataForJson[i].POLISH_Value;
				string tURKISH_Value = LocData.LDataForJson[i].TURKISH_Value;
				string uKRAINIAN_Value = LocData.LDataForJson[i].UKRAINIAN_Value;
				LocalizedInformation value = new LocalizedInformation(english_Value, pORTUGUESE_Value, fRENCH_Value, gERMAN_Value, rUSSIAN_Value, cHINESE_Value, sPANISH_Value, jAPANESE_Value, iTALIAN_Value, kOREAN_Value, cZECH_Value, pOLISH_Value, tURKISH_Value, uKRAINIAN_Value);
				if (AllTextData.ContainsKey(LocData.LDataForJson[i].TheKey))
				{
					UnityEngine.Debug.Log("KEY (" + LocData.LDataForJson[i].TheKey + ") ALREADY EXISTS FOR LOCALIZATION");
				}
				else
				{
					AllTextData.Add(LocData.LDataForJson[i].TheKey, value);
				}
			}
		}
		CheckIfAllStatsHaveTexts();
	}

	public void CheckIfAllStatsHaveTexts()
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (ItemStat stat in DatabaseManager.AllItemStats.Stats)
		{
			if (!AllTextData.ContainsKey(stat.Stat.VariableName + stat.Stat.StatsProp))
			{
				hashSet.Add(stat.Stat.VariableName + stat.Stat.StatsProp);
			}
		}
		foreach (CharacterStatInfo characterStat in DatabaseManager.CharacterStatList)
		{
			if (!AllTextData.ContainsKey(characterStat.Stat.VariableName + characterStat.Stat.StatsProp))
			{
				hashSet.Add(characterStat.Stat.VariableName + characterStat.Stat.StatsProp);
			}
		}
		foreach (GemStatInfo gemStat in DatabaseManager.GemStatList)
		{
			if (!AllTextData.ContainsKey(gemStat.stat.VariableName + gemStat.stat.StatsProp))
			{
				hashSet.Add(gemStat.stat.VariableName + gemStat.stat.StatsProp);
			}
		}
		foreach (TreeNodeInfo treeNode in MainTree.TreeNodes)
		{
			foreach (StatInfo item in treeNode.GetNodeStat())
			{
				if (!AllTextData.ContainsKey(item.VariableName + item.StatsProp))
				{
					hashSet.Add(item.VariableName + item.StatsProp);
				}
			}
		}
		foreach (ShinyInfo shiny in DatabaseManager.ShinyList)
		{
			if (!AllTextData.ContainsKey(shiny.MainStat.VariableName + shiny.MainStat.StatsProp))
			{
				hashSet.Add(shiny.MainStat.VariableName + shiny.MainStat.StatsProp);
			}
		}
		foreach (ShinyInfo shiny2 in DatabaseManager.ShinyList)
		{
			if (!AllTextData.ContainsKey(shiny2.ExtraStat.VariableName + shiny2.ExtraStat.StatsProp))
			{
				hashSet.Add(shiny2.ExtraStat.VariableName + shiny2.ExtraStat.StatsProp);
			}
		}
		foreach (WellInfo well in DatabaseManager.WellList)
		{
			foreach (StatInfo item2 in well.MainStat)
			{
				if (!AllTextData.ContainsKey(item2.VariableName + item2.StatsProp))
				{
					hashSet.Add(item2.VariableName + item2.StatsProp);
				}
			}
			foreach (StatInfo item3 in well.ExtraStat)
			{
				if (!AllTextData.ContainsKey(item3.VariableName + item3.StatsProp))
				{
					hashSet.Add(item3.VariableName + item3.StatsProp);
				}
			}
		}
		foreach (string item4 in hashSet)
		{
			UnityEngine.Debug.Log("STAT (" + item4 + ") DOES NOT HAVE A TEXT");
		}
	}

	public static string FormatTag(string tag, string color)
	{
		return "<size=110%><color=" + color + ">" + tag + "</color></size>";
	}

	public static void ChangeLanguage(int TheLanguage)
	{
		playerData.instance.ChoosenLanguage = (Languages)TheLanguage;
	}

	public static Languages GetSystemLangauge()
	{
		if (ManagerOfTheGame.instance.isMobile)
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.English:
				return Languages.English;
			case SystemLanguage.Portuguese:
				return Languages.Portuguese;
			case SystemLanguage.French:
				return Languages.French;
			case SystemLanguage.German:
				return Languages.German;
			case SystemLanguage.Russian:
				return Languages.Russian;
			case SystemLanguage.ChineseSimplified:
			case SystemLanguage.ChineseTraditional:
				return Languages.Chinese;
			case SystemLanguage.Spanish:
				return Languages.Spanish;
			case SystemLanguage.Japanese:
				return Languages.Japanese;
			case SystemLanguage.Italian:
				return Languages.Italian;
			case SystemLanguage.Korean:
				return Languages.Korean;
			default:
				return Languages.English;
			}
		}
		switch (SteamApps.GetCurrentGameLanguage())
		{
		case "english":
			return Languages.English;
		case "portuguese":
		case "brazilian":
			return Languages.Portuguese;
		case "french":
			return Languages.French;
		case "german":
			return Languages.German;
		case "russian":
			return Languages.Russian;
		case "schinese":
		case "tchinese":
			return Languages.Chinese;
		case "spanish":
		case "latam":
			return Languages.Spanish;
		case "japanese":
			return Languages.Japanese;
		case "italian":
			return Languages.Italian;
		case "koreana":
			return Languages.Korean;
		case "czech":
			return Languages.Czech;
		case "polish":
			return Languages.Polish;
		case "turkish":
			return Languages.Turkish;
		case "ukrainian":
			return Languages.Ukrainian;
		default:
			return Languages.English;
		}
	}

	public static string GetTranslatedValue(string TheKey)
	{
		string text = "";
		if (!AllTextData.ContainsKey(TheKey))
		{
			UnityEngine.Debug.Log("Key Not Found For Translation: " + TheKey);
			return TheKey;
		}
		if (playerData.instance.ChoosenLanguage == Languages.English)
		{
			text = AllTextData[TheKey].ENGLISH_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.French)
		{
			text = AllTextData[TheKey].FRENCH_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Spanish)
		{
			text = AllTextData[TheKey].SPANISH_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Chinese)
		{
			text = AllTextData[TheKey].CHINESE_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Portuguese)
		{
			text = AllTextData[TheKey].PORTUGUESE_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Russian)
		{
			text = AllTextData[TheKey].RUSSIAN_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Japanese)
		{
			text = AllTextData[TheKey].JAPANESE_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Italian)
		{
			text = AllTextData[TheKey].ITALIAN_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Korean)
		{
			text = AllTextData[TheKey].KOREAN_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.German)
		{
			text = AllTextData[TheKey].GERMAN_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Czech)
		{
			text = AllTextData[TheKey].CZECH_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Polish)
		{
			text = AllTextData[TheKey].POLISH_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Turkish)
		{
			text = AllTextData[TheKey].TURKISH_Value;
		}
		else if (playerData.instance.ChoosenLanguage == Languages.Ukrainian)
		{
			text = AllTextData[TheKey].UKRAINIAN_Value;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = AllTextData[TheKey].ENGLISH_Value;
			if (string.IsNullOrEmpty(text))
			{
				UnityEngine.Debug.LogWarning("[LocalizerManager] Missing translation for key '" + TheKey + "' in all languages");
				text = TheKey;
			}
		}
		return text;
	}

	private static string GetStringAndReplaceValues(string TheText, Dictionary<string, DoubleAndRoundToNearest> Values)
	{
		string text = TheText;
		foreach (string key in Values.Keys)
		{
			double number = Math.Round(Values[key].DoubleValue, Values[key].RoundToNearest);
			string newValue = Values[key].StartingSign + number.ToReadable(isForceReadable: false, Values[key].RoundToNearest);
			text = text.Replace(key, newValue);
		}
		return text;
	}

	private static string GetStringAndReplaceValues(string TheText, Dictionary<string, DoubleAndRoundToNearest> NumberValues, Dictionary<string, string> StringValues)
	{
		string text = TheText;
		foreach (string key in NumberValues.Keys)
		{
			double number = Math.Round(NumberValues[key].DoubleValue, NumberValues[key].RoundToNearest);
			string newValue = NumberValues[key].StartingSign + number.ToReadable(isForceReadable: false, NumberValues[key].RoundToNearest);
			text = text.Replace(key, newValue);
		}
		foreach (KeyValuePair<string, string> StringValue in StringValues)
		{
			text = text.Replace(StringValue.Key, StringValue.Value);
		}
		return text;
	}

	private static string GetStringAndReplaceValues(string TheText, Dictionary<string, DoubleAndRoundToNearest> NumberValues, string StringTag, string StringValue)
	{
		string text = TheText;
		foreach (string key in NumberValues.Keys)
		{
			double number = Math.Round(NumberValues[key].DoubleValue, NumberValues[key].RoundToNearest);
			string newValue = NumberValues[key].StartingSign + number.ToReadable(isForceReadable: false, NumberValues[key].RoundToNearest);
			text = text.Replace(key, newValue);
		}
		return text.Replace(StringTag, StringValue);
	}

	private static string GetStringAndReplaceValues(string TheText, string StringTag, string StringValue)
	{
		return TheText.Replace(StringTag, StringValue);
	}

	private static string GetStringAndReplaceValues(string TheText, DoubleAndRoundToNearest Value)
	{
		string newValue = string.Concat(str1: Math.Round(Value.DoubleValue, Value.RoundToNearest).ToReadable(isForceReadable: false, Value.RoundToNearest), str0: Value.StartingSign);
		return TheText.Replace("#VALUE#", newValue);
	}

	public static string GetTranslatedThenReplaceValues(string TheKey, Dictionary<string, string> Values)
	{
		string text = "";
		if (AllTextData.ContainsKey(TheKey + "_Plural"))
		{
			string text2 = Values[Values.ContainsKey("#VALUE#") ? "#VALUE#" : "#VALUE1#"];
			double? num = null;
			double result;
			if (FunctionsNeeded.HasColorTags(text2))
			{
				num = FunctionsNeeded.ExtractDoubleFromColorTags(text2);
			}
			else if (double.TryParse(text2, out result))
			{
				num = result;
			}
			if (num.HasValue && (num.Value > 1.0 || num.Value < -1.0))
			{
				TheKey += "_Plural";
			}
		}
		text = GetTranslatedValue(TheKey);
		foreach (KeyValuePair<string, string> Value in Values)
		{
			text = text.Replace(Value.Key, Value.Value);
		}
		return text;
	}

	public static string GetTranslatedThenReplaceValues(string TheKey, Dictionary<string, DoubleAndRoundToNearest> NumberValues, Dictionary<string, string> StringValues)
	{
		return GetStringAndReplaceValues(GetTranslatedValue(TheKey), NumberValues, StringValues);
	}

	public static string GetTranslatedThenReplaceValues(string TheKey, Dictionary<string, DoubleAndRoundToNearest> NumberValues, string StringTag, string StringValue)
	{
		return GetStringAndReplaceValues(GetTranslatedValue(TheKey), NumberValues, StringTag, StringValue);
	}

	public static string GetTranslatedThenReplaceValues(string TheKey, Dictionary<string, DoubleAndRoundToNearest> Values)
	{
		return GetStringAndReplaceValues(GetTranslatedValue(TheKey), Values);
	}

	public static string GetTranslatedThenReplaceValues(string TheKey, DoubleAndRoundToNearest Value)
	{
		if (!FunctionsNeeded.ApproximatelyEqualEpsilon(Value.DoubleValue, 1.0) && AllTextData.ContainsKey(TheKey + "_Plural"))
		{
			TheKey += "_Plural";
		}
		return GetStringAndReplaceValues(GetTranslatedValue(TheKey), Value);
	}

	public static string GetTranslatedThenReplaceValues(string TheKey, string StringTag, string StringValue)
	{
		if (AllTextData.ContainsKey(TheKey + "_Plural"))
		{
			double? num = null;
			double result;
			if (FunctionsNeeded.HasColorTags(StringValue))
			{
				num = FunctionsNeeded.ExtractDoubleFromColorTags(StringValue);
			}
			else if (double.TryParse(StringValue, out result))
			{
				num = result;
			}
			if (num.HasValue && (num.Value > 1.0 || num.Value < -1.0))
			{
				TheKey += "_Plural";
			}
		}
		return GetStringAndReplaceValues(GetTranslatedValue(TheKey), StringTag, StringValue);
	}

	public static string GetTranslatedThenReplaceValues(string TheKey, string StringValue)
	{
		if (AllTextData.ContainsKey(TheKey + "_Plural"))
		{
			double? num = null;
			double result;
			if (FunctionsNeeded.HasColorTags(StringValue))
			{
				num = FunctionsNeeded.ExtractDoubleFromColorTags(StringValue);
			}
			else if (double.TryParse(StringValue, out result))
			{
				num = result;
			}
			if (num.HasValue && (num.Value > 1.0 || num.Value < -1.0))
			{
				TheKey += "_Plural";
			}
		}
		return GetStringAndReplaceValues(GetTranslatedValue(TheKey), "#VALUE#", StringValue);
	}
}
public enum Languages
{
	English,
	Portuguese,
	French,
	German,
	Russian,
	Chinese,
	Spanish,
	Japanese,
	Italian,
	Korean,
	Czech,
	Polish,
	Turkish,
	Ukrainian
}
[Serializable]
public class LDataForJson
{
	public string TheKey;

	public string English_Value;

	public string PORTUGUESE_Value;

	public string FRENCH_Value;

	public string GERMAN_Value;

	public string RUSSIAN_Value;

	public string CHINESE_Value;

	public string SPANISH_Value;

	public string JAPANESE_Value;

	public string ITALIAN_Value;

	public string KOREAN_Value;

	public string CZECH_Value;

	public string POLISH_Value;

	public string TURKISH_Value;

	public string UKRAINIAN_Value;
}
[Serializable]
public class LocalizedInformation
{
	public string ENGLISH_Value;

	public string PORTUGUESE_Value;

	public string FRENCH_Value;

	public string GERMAN_Value;

	public string RUSSIAN_Value;

	public string CHINESE_Value;

	public string SPANISH_Value;

	public string JAPANESE_Value;

	public string ITALIAN_Value;

	public string KOREAN_Value;

	public string CZECH_Value;

	public string POLISH_Value;

	public string TURKISH_Value;

	public string UKRAINIAN_Value;

	public LocalizedInformation(string Eng, string pr, string Fren, string ger, string rus, string chin, string spa, string jpa, string ita, string kor, string cze, string pol, string tur, string ukr)
	{
		ENGLISH_Value = Eng;
		PORTUGUESE_Value = pr;
		FRENCH_Value = Fren;
		GERMAN_Value = ger;
		RUSSIAN_Value = rus;
		CHINESE_Value = chin;
		SPANISH_Value = spa;
		JAPANESE_Value = jpa;
		ITALIAN_Value = ita;
		KOREAN_Value = kor;
		CZECH_Value = cze;
		POLISH_Value = pol;
		TURKISH_Value = tur;
		UKRAINIAN_Value = ukr;
	}
}
[Serializable]
public class LocalizationDataList
{
	public List<LDataForJson> LDataForJson = new List<LDataForJson>();
}
[Serializable]
public class DoubleAndRoundToNearest
{
	public double DoubleValue;

	public int RoundToNearest;

	public string StartingSign;

	public DoubleAndRoundToNearest(double dValue, int roundToNearest, string sign)
	{
		DoubleValue = dValue;
		RoundToNearest = roundToNearest;
		StartingSign = sign;
	}
}
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	protected static bool s_EverInitialized;

	protected static SteamManager s_instance;

	protected bool m_bInitialized;

	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	protected static SteamManager Instance
	{
		get
		{
			if (s_instance == null)
			{
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			}
			return s_instance;
		}
	}

	public static bool Initialized => Instance.m_bInitialized;

	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		UnityEngine.Debug.LogWarning(pchDebugText);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitOnPlayMode()
	{
		s_EverInitialized = false;
		s_instance = null;
	}

	protected virtual void Awake()
	{
		if (s_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		s_instance = this;
		if (s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			UnityEngine.Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			UnityEngine.Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary((AppId_t)4005560u))
			{
				UnityEngine.Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			UnityEngine.Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + ex, this);
			Application.Quit();
			return;
		}
		m_bInitialized = SteamAPI.Init();
		if (!m_bInitialized)
		{
			UnityEngine.Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
		}
		else
		{
			s_EverInitialized = true;
		}
	}

	protected virtual void OnEnable()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		if (m_bInitialized && m_SteamAPIWarningMessageHook == null)
		{
			m_SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
		}
	}

	protected virtual void OnDestroy()
	{
		if (!(s_instance != this))
		{
			s_instance = null;
			if (m_bInitialized)
			{
				SteamAPI.Shutdown();
			}
		}
	}

	protected virtual void Update()
	{
		if (m_bInitialized)
		{
			SteamAPI.RunCallbacks();
		}
	}
}
public class testArcherAim : MonoBehaviour
{
	private Bone targetBone;

	private void Start()
	{
		targetBone = GetComponent<SkeletonAnimation>().skeleton.FindBone("target");
	}

	private void Update()
	{
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		position.z = 0f;
		Vector2 position2 = base.transform.InverseTransformPoint(position);
		targetBone.SetLocalPosition(position2);
	}
}
[CreateAssetMenu]
public class AzrarInfo : SerializedScriptableObject
{
	[HideInInspector]
	public string FunctionName;

	public int MaxLevel;

	public Sprite Icon;

	public bool isSpecialAzrar;

	public List<string> ValueEquation;

	public List<string> CostEquation;

	public List<Currencies> CostCurrencies;

	public List<StatInfo> Stats;

	public List<AzrarCondition> Conditions;

	public int LevelToShowRed;
}
[Serializable]
public abstract class AzrarCondition
{
	public abstract bool IsMet();
}
public class Cond_PrevAzrar : AzrarCondition
{
	public AzrarInfo Azrar;

	public int Level = 1;

	public override bool IsMet()
	{
		return playerData.instance.AzrarLevels[Azrar.FunctionName] >= Level;
	}
}
public class Cond_PlayerLevel : AzrarCondition
{
	public int PlayerLevel;

	public override bool IsMet()
	{
		return playerData.instance.PlayerLevel >= PlayerLevel;
	}
}
public class Cond_MonstersLevel : AzrarCondition
{
	public int MonstersLevel;

	public override bool IsMet()
	{
		return playerData.instance.MonstersLevel >= MonstersLevel;
	}
}
public class Cond_TotalCurrency : AzrarCondition
{
	public Currencies Currency;

	public double Amount;

	public override bool IsMet()
	{
		return playerData.instance.TotalCurrenciesGained_CurrentRun[Currency] >= Amount;
	}
}
public class Cond_AlwaysMet : AzrarCondition
{
	public override bool IsMet()
	{
		return true;
	}
}
public class Cond_SystemUnlocked : AzrarCondition
{
	public UnlockableSystems System;

	public override bool IsMet()
	{
		return playerData.instance.UnlockedSystems[System];
	}
}
public class Cond_SkillUnlocked : AzrarCondition
{
	public string SkillName;

	public override bool IsMet()
	{
		if (!playerData.instance.UnlockedSystems[UnlockableSystems.Skills])
		{
			return false;
		}
		if (SkillName == "")
		{
			return true;
		}
		return playerData.instance.SkillIsUnlocked[SkillName];
	}
}
public class AzrarManager : MonoBehaviour
{
	public static AzrarManager instance;

	public GameObject AzrarParent;

	public GameObject AzrarPrefab;

	private Dictionary<AzrarInfo, bool> AzrarIsShown = new Dictionary<AzrarInfo, bool>();

	public Action OnAzrarUnlocked;

	public GameObject LevelAndExpBar;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		foreach (AzrarInfo azrar in DatabaseManager.AzrarList)
		{
			AzrarIsShown.Add(azrar, value: false);
		}
		if (playerData.instance.MonstersLevel > 2)
		{
			ShowOneAzrar(DatabaseManager.AzrarDict["Damage"]);
		}
		CheckAzrarUnlocks(isDelayed: false);
		ShowUnlockedAzrars();
		LevelAndExpBar.SetActive(playerData.instance.UnlockedSystems[UnlockableSystems.Skills]);
	}

	public IEnumerator ClickedOnEndRun()
	{
		foreach (Transform item in AzrarParent.transform)
		{
			item.GetComponent<Breather>().SetSizeToZero();
			item.gameObject.SetActive(value: false);
		}
		yield return new WaitForSeconds(0.25f);
		foreach (Transform item2 in AzrarParent.transform)
		{
			item2.GetComponent<AzrarSelfer>().UpdateUI();
			item2.gameObject.SetActive(value: true);
			item2.GetComponent<Breather>().Pop();
		}
	}

	public void CheckAzrarUnlocks(bool isDelayed)
	{
		foreach (AzrarInfo azrar in DatabaseManager.AzrarList)
		{
			if (!playerData.instance.AzrarIsUnlocked[azrar.FunctionName] && (azrar.Conditions.All((AzrarCondition condition) => condition.IsMet()) || isShowForRed(azrar)) && (!(azrar.FunctionName == "GainWellCurrency") || playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount))
			{
				playerData.instance.AzrarIsUnlocked[azrar.FunctionName] = true;
				playerData.instance.AzrarLevels[azrar.FunctionName] = 0;
				if (isDelayed)
				{
					StartCoroutine(DelayedShowOneAzrar(azrar));
				}
				else
				{
					ShowOneAzrar(azrar);
				}
			}
		}
	}

	private bool isShowForRed(AzrarInfo azrar)
	{
		if (azrar.LevelToShowRed > 0 && azrar.Conditions.Any((AzrarCondition condition) => condition is Cond_MonstersLevel))
		{
			if (azrar.Conditions.Where((AzrarCondition condition) => !(condition is Cond_MonstersLevel)).All((AzrarCondition condition) => condition.IsMet()))
			{
				return playerData.instance.MonstersLevel >= azrar.LevelToShowRed;
			}
			return false;
		}
		return false;
	}

	private void ShowUnlockedAzrars()
	{
		foreach (AzrarInfo azrar in DatabaseManager.AzrarList)
		{
			if (playerData.instance.AzrarIsUnlocked[azrar.FunctionName] && playerData.instance.AzrarLevels[azrar.FunctionName] < azrar.MaxLevel && !AzrarIsShown[azrar])
			{
				ShowOneAzrar(azrar);
			}
		}
	}

	private IEnumerator DelayedShowOneAzrar(AzrarInfo azrar)
	{
		yield return new WaitForSeconds(0.01f);
		ShowOneAzrar(azrar);
	}

	private void ShowOneAzrar(AzrarInfo azrar)
	{
		if (playerData.instance.AzrarLevels[azrar.FunctionName] < azrar.MaxLevel)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(AzrarPrefab, AzrarParent.transform);
			gameObject.GetComponent<AzrarSelfer>().TakeInfo(azrar);
			AzrarIsShown[azrar] = true;
			if (azrar.isSpecialAzrar && azrar.Conditions.All((AzrarCondition condition) => condition.IsMet()))
			{
				gameObject.transform.SetAsFirstSibling();
			}
		}
	}

	public void ApplySpecialAzrar(AzrarInfo azrar)
	{
		if (azrar.FunctionName == "Tree")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Tree] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Tree] = true;
			MainMenusManager.instance.ClickedOnButton(0);
			TutorialManager.instance.DelayedShowTutorial();
			MainMenusManager.instance.CheckButtons();
			AchievementsManager.instance.UnlockAchievement("Tree_Unlocked");
		}
		else if (azrar.FunctionName == "Skills")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Skills] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Skills] = true;
			MainMenusManager.instance.ClickedOnButton(1);
			TutorialManager.instance.DelayedShowTutorial();
			LevelAndExpBar.SetActive(value: true);
			MainMenusManager.instance.CheckButtons();
		}
		else if (azrar.FunctionName == "Shiny")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Shiny] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Shiny] = true;
			MainMenusManager.instance.ClickedOnButton(2);
			playerData.instance.stats.ChangeAStat("ShinyChance", StatsProperties.Flat, DatabaseManager.BaseShinyChance, IsAdd: true);
			TutorialManager.instance.DelayedShowTutorial();
			MainMenusManager.instance.CheckButtons();
		}
		else if (azrar.FunctionName == "Items")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Items] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Items] = true;
			MainMenusManager.instance.ClickedOnButton(3);
			TutorialManager.instance.DelayedShowTutorial();
			playerData.instance.stats.ChangeAStat("ItemsChance", StatsProperties.Flat, DatabaseManager.BaseItemsChance, IsAdd: true);
			MainMenusManager.instance.CheckButtons();
		}
		else if (azrar.FunctionName == "Heroes")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Heroes] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Heroes] = true;
			MainMenusManager.instance.ClickedOnButton(4);
			TutorialManager.instance.DelayedShowTutorial();
			playerData.instance.stats.ChangeAStat("GhostChance", StatsProperties.Flat, DatabaseManager.BaseGhostChance, IsAdd: true);
			playerData.instance.stats.ChangeAStat("CharacterCurrencyDrop", StatsProperties.Flat, DatabaseManager.BaseGhostDrop_CharacterCurrency, IsAdd: true);
			MainMenusManager.instance.CheckButtons();
			CharacterUIManager.instance.UpdateDamageText();
			AchievementsManager.instance.UnlockAchievement("UnlockArcher");
		}
		else if (azrar.FunctionName == "Gems")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Gems] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Gems] = true;
			playerData.instance.GemsIsUnlocked[0] = true;
			GemsManager.instance.UnlockedTheSystem();
			MainMenusManager.instance.ClickedOnButton(5);
			TutorialManager.instance.DelayedShowTutorial();
			playerData.instance.stats.ChangeAStat("OreChance", StatsProperties.Flat, DatabaseManager.BaseOreChance, IsAdd: true);
			playerData.instance.stats.ChangeAStat("OreChance_Rich", StatsProperties.Flat, DatabaseManager.BaseOreChance_Rich, IsAdd: true);
			playerData.instance.stats.ChangeAStat("OreDrop_GemCurrency", StatsProperties.Flat, DatabaseManager.BaseOreDrop_GemCurrency, IsAdd: true);
			playerData.instance.stats.ChangeAStat("RichOreMultiplier", StatsProperties.Flat, DatabaseManager.RichOreMultiplier, IsAdd: true);
			MainMenusManager.instance.CheckButtons();
		}
		else if (azrar.FunctionName == "Bounties")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Bounties] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Bounties] = true;
			playerData.instance.stats.ChangeAStat("ChanceToSpawnBounty", StatsProperties.Flat, DatabaseManager.BaseChanceToSpawnBounty, IsAdd: true);
			TutorialManager.instance.CheckButtons();
			TutorialManager.instance.DelayedShowTutorial();
			TutorialManager.instance.ClickedOnButton(6);
		}
		else if (azrar.FunctionName == "TreasureChest")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.TreasureChest] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.TreasureChest] = true;
			playerData.instance.stats.ChangeAStat("TreasureChance", StatsProperties.Flat, DatabaseManager.BaseTreasureChance, IsAdd: true);
			playerData.instance.stats.ChangeAStat("TreasureGoldMultiplier", StatsProperties.Flat, DatabaseManager.BaseTreasureGoldMultiplier, IsAdd: true);
			playerData.instance.stats.ChangeAStat("TreasureExpMultiplier", StatsProperties.Flat, DatabaseManager.BaseTreasureExpMultiplier, IsAdd: true);
			TutorialManager.instance.CheckButtons();
			TutorialManager.instance.DelayedShowTutorial();
			TutorialManager.instance.ClickedOnButton(7);
		}
		else if (azrar.FunctionName == "Well")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Well] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Well] = true;
			playerData.instance.MonsterLevelWhenWellReset = playerData.instance.MonstersLevel;
			MainMenusManager.instance.ClickedOnButton(8);
			TutorialManager.instance.DelayedShowTutorial();
			MainMenusManager.instance.CheckButtons();
			WellManager.instance.UnlockedWellSystem();
		}
		else if (azrar.FunctionName == "Towers")
		{
			playerData.instance.UnlockedSystems[UnlockableSystems.Towers] = true;
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Towers] = true;
			TutorialManager.instance.DelayedShowTutorial();
			TutorialManager.instance.CheckButtons();
			TutorialManager.instance.DelayedShowTutorial();
			TutorialManager.instance.ClickedOnButton(9);
		}
		EnemiesManager.instance.ForceClearEnemiesInPreviousRun();
		CheckAzrarUnlocks(isDelayed: true);
	}
}
public class AzrarSelfer : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	private AzrarInfo MyInfo;

	public TextMeshProUGUI DescriptionText;

	public TextMeshProUGUI LevelText;

	public TextMeshProUGUI CostText;

	public Image Icon;

	public Color PurchaseableColor;

	public Color UnpurchaseableColor;

	public Color DescriptionTextColor_Red;

	public Color DescriptionTextColor_White;

	public GameObject Blackout;

	private List<double> costs = new List<double>();

	private bool isCanSpend;

	private List<bool> isCanSpendCurrencies = new List<bool>();

	private float scale_OnEnter = 1.03f;

	private float duration_OnEnter = 0.2f;

	private Ease ease_OnEnter = Ease.OutBack;

	public void TakeInfo(AzrarInfo info)
	{
		MyInfo = info;
		UpdateUI();
		foreach (Currencies costCurrency in MyInfo.CostCurrencies)
		{
			Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
			Currencies key = costCurrency;
			onCurrencyChange[key] = (Action)Delegate.Combine(onCurrencyChange[key], new Action(ManageCosts));
		}
	}

	private void OnEnable()
	{
	}

	public void UpdateUI()
	{
		int num = playerData.instance.AzrarLevels[MyInfo.FunctionName];
		_ = MyInfo.MaxLevel;
		Icon.sprite = MyInfo.Icon;
		FunctionsNeeded.ConstrainImageSize(Icon.rectTransform, Icon, 35f, 35f);
		DescriptionText.text = "";
		if (MyInfo.FunctionName == "Damage")
		{
			DescriptionText.color = DescriptionTextColor_White;
			for (int i = 0; i < MyInfo.ValueEquation.Count; i++)
			{
				double item = ExpressionEvaluator.Evaluate(MyInfo.ValueEquation[i], (num <= 0) ? 1 : num);
				if (MyInfo.FunctionName == "Damage")
				{
					item = DatabaseManager.DamageValue(num + 1);
				}
				DescriptionText.text += MyInfo.Stats[i].GetValueDescText_SingleOrMultipleValues(new List<double> { item }, isColoredTag: false);
				if (i < MyInfo.ValueEquation.Count - 1)
				{
					DescriptionText.text += "\n";
				}
			}
		}
		else if (MyInfo.FunctionName == "GainRune")
		{
			DescriptionText.color = DescriptionTextColor_White;
			DescriptionText.text = LocalizerManager.GetTranslatedValue("GainRune_AzrarDesc");
		}
		else if (MyInfo.FunctionName == "SpawnShiny")
		{
			DescriptionText.color = DescriptionTextColor_White;
			DescriptionText.text = LocalizerManager.GetTranslatedThenReplaceValues("SpawnShiny_AzrarDesc", "3");
		}
		else if (MyInfo.FunctionName == "SpawnOre")
		{
			DescriptionText.color = DescriptionTextColor_White;
			DescriptionText.text = LocalizerManager.GetTranslatedThenReplaceValues("SpawnOre_AzrarDesc", "3");
		}
		else if (MyInfo.FunctionName == "SpawnGhost")
		{
			DescriptionText.color = DescriptionTextColor_White;
			DescriptionText.text = LocalizerManager.GetTranslatedValue("SpawnGhost_AzrarDesc");
		}
		else if (isRed())
		{
			DescriptionText.color = DescriptionTextColor_Red;
			DescriptionText.text = LocalizerManager.GetTranslatedValue("Requirement_Text") + ":\n" + LocalizerManager.GetTranslatedThenReplaceValues("MonstersLevel_Text", ((Cond_MonstersLevel)MyInfo.Conditions.First((AzrarCondition condition) => condition is Cond_MonstersLevel)).MonstersLevel.ToString());
		}
		else
		{
			DescriptionText.color = DescriptionTextColor_White;
			DescriptionText.text = LocalizerManager.GetTranslatedValue(MyInfo.FunctionName + "_AzrarDesc");
		}
		ManageCosts();
	}

	private bool isRed()
	{
		if (MyInfo.LevelToShowRed > 0 && playerData.instance.MonstersLevel >= MyInfo.LevelToShowRed && MyInfo.Conditions.Any((AzrarCondition condition) => condition is Cond_MonstersLevel) && !MyInfo.Conditions.All((AzrarCondition condition) => condition.IsMet()))
		{
			return true;
		}
		return false;
	}

	public void ManageCosts()
	{
		int num = playerData.instance.AzrarLevels[MyInfo.FunctionName];
		costs.Clear();
		for (int i = 0; i < MyInfo.CostEquation.Count; i++)
		{
			double item = ExpressionEvaluator.Evaluate(MyInfo.CostEquation[i], num + 1);
			costs.Add(item);
		}
		if (MyInfo.FunctionName == "Damage")
		{
			costs[0] = DatabaseManager.DamageCost(num + 1);
		}
		else if (MyInfo.FunctionName == "GainWellCurrency")
		{
			costs[0] = DatabaseManager.GainWellCurrency_Azrar_Cost(num + 1);
		}
		else if (MyInfo.FunctionName == "SpawnShiny")
		{
			costs[0] = DatabaseManager.SpawnShiny_Azrar_Cost(num + 1);
		}
		else if (MyInfo.FunctionName == "SpawnOre")
		{
			costs[0] = DatabaseManager.SpawnOre_Azrar_Cost(num + 1);
		}
		else if (MyInfo.FunctionName == "SpawnGhost")
		{
			costs[0] = DatabaseManager.SpawnGhost_Azrar_Cost(num + 1);
		}
		CostText.text = "";
		(bool, List<bool>) tuple = PlayerManager.instance.IsCanSpendCurrencies(costs, MyInfo.CostCurrencies, isSpendAlso: false);
		isCanSpend = tuple.Item1;
		isCanSpendCurrencies = tuple.Item2;
		for (int j = 0; j < costs.Count; j++)
		{
			CostText.text += ("<sprite name=" + MyInfo.CostCurrencies[j].ToString() + ">" + costs[j].ToReadable()).ToColored(isCanSpendCurrencies[j] ? PurchaseableColor : UnpurchaseableColor);
			if (j < costs.Count - 1)
			{
				CostText.text += "\n";
			}
		}
		if (isRed())
		{
			isCanSpend = false;
			CostText.text = "";
		}
		Blackout.SetActive(!isCanSpend);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isCanSpend)
		{
			base.transform.localScale = Vector3.one;
			base.transform.DOScale(scale_OnEnter, duration_OnEnter).SetEase(ease_OnEnter);
			base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			DOTween.Kill(base.gameObject.GetHashCode() + 3);
			base.transform.DOPunchRotation(Vector3.forward * 4f, 0.1f, 10, 0.5f).SetId(base.gameObject.GetHashCode() + 3);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.DOScale(1f, duration_OnEnter).SetEase(ease_OnEnter);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (isRed())
		{
			return;
		}
		ManageCosts();
		(isCanSpend, isCanSpendCurrencies) = PlayerManager.instance.IsCanSpendCurrencies(costs, MyInfo.CostCurrencies, isSpendAlso: true);
		if (isCanSpend)
		{
			if (MyInfo.isSpecialAzrar)
			{
				AzrarManager.instance.ApplySpecialAzrar(MyInfo);
			}
			else if (MyInfo.FunctionName == "Damage")
			{
				ApplyRemoveStat(playerData.instance.AzrarLevels[MyInfo.FunctionName], playerData.instance.AzrarLevels[MyInfo.FunctionName] + 1, isAdd: true);
			}
			else if (MyInfo.FunctionName == "GainWellCurrency")
			{
				WellManager.instance.GainWellCurrency_Azrar();
			}
			else if (MyInfo.FunctionName == "SpawnShiny")
			{
				EnemiesManager.instance.AddAForcedSpawnEnemy_FromAzrar(EnemiesManager.ForceMonsterSpawnType.Shiny);
			}
			else if (MyInfo.FunctionName == "SpawnOre")
			{
				EnemiesManager.instance.AddAForcedSpawnEnemy_FromAzrar(EnemiesManager.ForceMonsterSpawnType.Ore);
			}
			else if (MyInfo.FunctionName == "SpawnGhost")
			{
				EnemiesManager.instance.AddAForcedSpawnEnemy_FromAzrar(EnemiesManager.ForceMonsterSpawnType.Ghost);
			}
			playerData.instance.AzrarLevels[MyInfo.FunctionName]++;
			UpdateUI();
			AzrarManager.instance.CheckAzrarUnlocks(isDelayed: true);
			AzrarManager.instance.OnAzrarUnlocked?.Invoke();
			FXManager.instance.PlayGeneralSound(GeneralSounds.purchase);
			if (playerData.instance.AzrarLevels[MyInfo.FunctionName] >= MyInfo.MaxLevel)
			{
				ReachedMaxLevel();
			}
		}
	}

	private void ApplyRemoveStat(int previousLevel, int newLevel, bool isAdd)
	{
		for (int i = 0; i < MyInfo.ValueEquation.Count; i++)
		{
			double num = 0.0;
			if (MyInfo.FunctionName == "Damage")
			{
				playerData.instance.stats.ChangeAStat(MyInfo.Stats[i].VariableName, MyInfo.Stats[i].StatsProp, DatabaseManager.DamageValue(newLevel), isAdd);
				continue;
			}
			num = ExpressionEvaluator.Evaluate(MyInfo.ValueEquation[i], previousLevel);
			double value = ExpressionEvaluator.Evaluate(MyInfo.ValueEquation[i], newLevel) - ((previousLevel > 0) ? num : 0.0);
			playerData.instance.stats.ChangeAStat(MyInfo.Stats[i].VariableName, MyInfo.Stats[i].StatsProp, value, isAdd);
		}
	}

	private void ReachedMaxLevel()
	{
		foreach (Currencies costCurrency in MyInfo.CostCurrencies)
		{
			Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
			Currencies key = costCurrency;
			onCurrencyChange[key] = (Action)Delegate.Remove(onCurrencyChange[key], new Action(ManageCosts));
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
public class CharacterSelfer : MonoBehaviour
{
	private bool IsSpecialAttack;

	private MeshRenderer characterRenderer;

	[HideInInspector]
	public bool IsAttacking;

	private bool IsTeleporting;

	private float AttackTimer = 100f;

	private Transform MyGFXTrans;

	private EnemySelfer CurrentTarget_ES;

	private Transform CurrentTarget_Trans;

	private SkeletonAnimation characterAnimator;

	private List<float> BaseAttackAnimationDuration = new List<float> { 0f, 0f };

	private List<float> CooldownBetweenAttacks = new List<float> { 0f, 0f };

	private float baseTimeScale;

	private bool IsAttackAnimationFinished = true;

	private List<float> AttackAnimationTimeScale = new List<float> { 0f, 0f };

	public Transform FirePoint;

	private Bone targetBone;

	private Vector2 PosToTeleportTo;

	private Coroutine TeleportingCo;

	private bool IsStartFight;

	private bool isInit;

	private Dictionary<string, float> ProjectileChance = new Dictionary<string, float>
	{
		{ "Archer_Poison", 0f },
		{ "Archer_Explosive", 0f },
		{ "Archer_Ice", 0f },
		{ "Archer_Normal", 100f }
	};

	public void Init()
	{
		if (!isInit)
		{
			isInit = true;
			IsAttackAnimationFinished = true;
			IsAttacking = false;
			MyGFXTrans = base.transform.Find("GFX");
			characterRenderer = MyGFXTrans.GetComponent<MeshRenderer>();
			characterAnimator = MyGFXTrans.GetComponent<SkeletonAnimation>();
			characterAnimator.AnimationState.Event += OnEventAnimation;
			characterAnimator.AnimationState.Complete += OnAnimationComplete;
			targetBone = characterAnimator.skeleton.FindBone("target");
			characterAnimator.timeScale = baseTimeScale;
			BaseAttackAnimationDuration[0] = characterAnimator.skeletonDataAsset.GetSkeletonData(quiet: true).FindAnimation("attack").Duration;
			BaseAttackAnimationDuration[1] = characterAnimator.skeletonDataAsset.GetSkeletonData(quiet: true).FindAnimation("attack_special").Duration;
			UpdateAttackAnimationTimeScale(playerData.instance.stats.Archer_AttackSpeed.Total.RealValue);
			AttackTimer = CooldownBetweenAttacks[0];
			StartIdleAnimation();
		}
	}

	public void StartRun()
	{
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Heroes])
		{
			Init();
			IsStartFight = false;
			MyGFXTrans.position = new Vector2(3000f, 3000f);
			UpdateAttackAnimationTimeScale(playerData.instance.stats.Archer_AttackSpeed.Total.RealValue);
			ProjectileChance["Archer_Poison"] = playerData.instance.stats.Archer_ChanceForPoison.Total.RealValue;
			ProjectileChance["Archer_Explosive"] = playerData.instance.stats.Archer_ChanceForExplosive.Total.RealValue;
			ProjectileChance["Archer_Ice"] = playerData.instance.stats.Archer_ChanceForIce.Total.RealValue;
			ProjectileChance["Archer_Normal"] = 100f - ProjectileChance["Archer_Poison"] - ProjectileChance["Archer_Explosive"] - ProjectileChance["Archer_Ice"];
			StartCoroutine(StartRunCoroutine());
		}
	}

	private IEnumerator StartRunCoroutine()
	{
		yield return new WaitForSeconds(1f);
		IsStartFight = true;
		GetANewTarget();
	}

	public void EndRun()
	{
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Heroes])
		{
			MyGFXTrans.position = new Vector2(3000f, 3000f);
			IsStartFight = false;
			StopAttacking();
		}
	}

	private void OnEventAnimation(TrackEntry trackEntry, Event e)
	{
		if (e.Data.Name == "attack" || e.Data.Name == "attack_special")
		{
			Fire();
		}
	}

	public void OnAnimationComplete(TrackEntry trackEntry)
	{
		if (trackEntry.Animation.ToString() == "attack" || trackEntry.Animation.ToString() == "attack_special")
		{
			IsAttackAnimationFinished = true;
		}
	}

	public void StartAttackAnimation()
	{
		if (CurrentTarget_ES != null)
		{
			Vector2 position = characterAnimator.transform.InverseTransformPoint(CurrentTarget_Trans.position);
			targetBone.SetLocalPosition(position);
		}
		int num = (FunctionsNeeded.IsHappened(50f) ? 1 : 0);
		characterAnimator.timeScale = AttackAnimationTimeScale[num];
		characterAnimator.AnimationState.SetAnimation(0, "attack" + ((num == 1) ? "_special" : ""), loop: false);
		IsSpecialAttack = num == 1;
	}

	private bool Fire()
	{
		if (CurrentTarget_ES == null || CurrentTarget_ES.isDead)
		{
			return false;
		}
		ProjectileInfo projInfo = DatabaseManager.ProjectileDict[FunctionsNeeded.GetARandomFromDict_Normal(ProjectileChance)];
		int num = ((!FunctionsNeeded.IsHappened(playerData.instance.stats.Well_ArcherChanceForDoubleArrows.Total.RealValue)) ? 1 : 2);
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			float num3 = FunctionsNeeded.CalculateAngle((Vector2)CurrentTarget_Trans.position - (Vector2)FirePoint.position, IsRadian: true);
			Vector2 vector = new Vector2(Mathf.Cos(num3), Mathf.Sin(num3));
			ProjectilesManager.instance.FireProjectile(FirePoint.position, FirePoint.position + (Vector3)vector * 1000f, projInfo, MultipleProjectileFormation.GMP, 1 + FunctionsNeeded.IsHappened_Over100Things(playerData.instance.stats.Archer_AdditionalProjectiles.Total.RealValue), num2, isFiredFromTowerDirectly: true);
			if (FunctionsNeeded.IsHappened(playerData.instance.stats.Well_ArcherChanceForShootFromBehind.Total.RealValue))
			{
				vector = new Vector2(Mathf.Cos(num3 + MathF.PI), Mathf.Sin(num3 + MathF.PI));
				ProjectilesManager.instance.FireProjectile(FirePoint.position, FirePoint.position + (Vector3)vector * 1000f, projInfo, MultipleProjectileFormation.GMP, 1 + FunctionsNeeded.IsHappened_Over100Things(playerData.instance.stats.Archer_AdditionalProjectiles.Total.RealValue), num2, isFiredFromTowerDirectly: true);
			}
			num2 += 150f;
		}
		return true;
	}

	private void StartIdleAnimation()
	{
		if (!IsAttacking)
		{
			IsAttackAnimationFinished = true;
			characterAnimator.timeScale = 1f;
			characterAnimator.AnimationState.SetAnimation(0, "idle", loop: true);
		}
	}

	public void UpdateAttackAnimationTimeScale(float AttackPerSecond)
	{
		for (int i = 0; i < BaseAttackAnimationDuration.Count; i++)
		{
			float num = 1f / AttackPerSecond;
			if (num > BaseAttackAnimationDuration[i])
			{
				CooldownBetweenAttacks[i] = num - BaseAttackAnimationDuration[i];
				AttackAnimationTimeScale[i] = 1f;
			}
			else
			{
				CooldownBetweenAttacks[i] = 0f;
				AttackAnimationTimeScale[i] = BaseAttackAnimationDuration[i] / num;
			}
		}
	}

	public void StartAttacking()
	{
		IsAttacking = true;
		IsAttackAnimationFinished = true;
		AttackTimer = 100f;
	}

	private void AttackFunction()
	{
		if (IsAttackAnimationFinished)
		{
			AttackTimer += Time.deltaTime;
			if (AttackTimer >= CooldownBetweenAttacks[IsSpecialAttack ? 1 : 0])
			{
				AttackTimer = 0f;
				IsAttackAnimationFinished = false;
				StartAttackAnimation();
			}
		}
	}

	private void StopAttacking()
	{
		IsAttackAnimationFinished = false;
		IsAttacking = false;
	}

	private IEnumerator TeleportToTarget()
	{
		StopAttacking();
		IsTeleporting = true;
		FXManager.instance.PlaySound("ArcherSmokeSound", ForcePlay: true);
		FXManager.instance.SpawnGFX("ArcherSmoke", MyGFXTrans.position + new Vector3(0f, 20f, 0f), 1f, Vector3.one * -1f);
		MyGFXTrans.position = new Vector2(3000f, 3000f);
		yield return new WaitForSeconds(0.2f);
		IsTeleporting = false;
		if (CurrentTarget_ES != null)
		{
			base.transform.position = PosToTeleportTo;
			MyGFXTrans.localPosition = Vector3.zero;
			Vector2 vector = MyGFXTrans.localScale;
			vector.x = Mathf.Abs(vector.x) * Mathf.Sign(CurrentTarget_Trans.position.x - base.transform.position.x) * -1f;
			MyGFXTrans.localScale = vector;
			StartAttacking();
			characterRenderer.sortingOrder = Mathf.RoundToInt((0f - MyGFXTrans.position.y) * 10f);
		}
	}

	private void GetANewTarget()
	{
		StopAttacking();
		CurrentTarget_ES = EnemiesManager.instance.GetRandomEnemy();
		if (CurrentTarget_ES != null)
		{
			CurrentTarget_Trans = CurrentTarget_ES.transform;
			CurrentTarget_ES.isStopUntillNextRun = true;
			CurrentTarget_ES.ForceStop();
		}
		PosToTeleportTo = GetPositionNextToTarget();
		if (TeleportingCo != null)
		{
			StopCoroutine(TeleportingCo);
		}
		TeleportingCo = StartCoroutine(TeleportToTarget());
	}

	private Vector3 GetPositionNextToTarget()
	{
		float num = -1f * Mathf.Sign(CurrentTarget_Trans.position.x - EnemiesManager.instance.Trapezoid_Center.x);
		Vector3 vector = CurrentTarget_Trans.position + new Vector3(num * (float)UnityEngine.Random.Range(300, 450), UnityEngine.Random.Range(-70, 70), 0f);
		vector.y = Mathf.Clamp(vector.y, EnemiesManager.instance.Trapezoid_BottomLeft_Transform.position.y, EnemiesManager.instance.Trapezoid_TopLeft_Transform.position.y);
		if (!EnemiesManager.instance.IsPointInTrapezoid(vector))
		{
			return CurrentTarget_Trans.position;
		}
		return vector;
	}

	private void Update()
	{
		if (RunManager.instance.isRunStarted && IsStartFight && playerData.instance.UnlockedSystems[UnlockableSystems.Heroes])
		{
			if (CurrentTarget_ES == null || CurrentTarget_ES.isDead)
			{
				GetANewTarget();
			}
			else if (IsAttacking)
			{
				AttackFunction();
			}
		}
	}
}
[CreateAssetMenu]
public class CharacterStatInfo : ScriptableObject
{
	[HideInInspector]
	public string FunctionName;

	public Sprite Icon;

	public StatInfo Stat;

	public int OrderOfAppearance;

	public int UnlockLevel;

	public int MaxLevel;

	public string ValueEquation;

	public string CostEquation;
}
public class CharacterUIManager : MonoBehaviour
{
	public static CharacterUIManager instance;

	public Transform StatsParent;

	private Dictionary<string, Transform> StatTransforms = new Dictionary<string, Transform>();

	private Dictionary<string, Image> Icons = new Dictionary<string, Image>();

	private Dictionary<string, TextMeshProUGUI> Description = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, TextMeshProUGUI> Costs = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, TextMeshProUGUI> LevelTexts = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, Button> LevelStatButtons = new Dictionary<string, Button>();

	private Dictionary<string, GameObject> LevelStatGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> IconLockedGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> IconUnlockedGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> DescImageLockedGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> DescImageUnlockedGOs = new Dictionary<string, GameObject>();

	public Color DescTextLockedColor;

	public Color DescTextUnlockedColor;

	public TextMeshProUGUI DamageText;

	public Button LevelCharacterButton;

	public TextMeshProUGUI LevelCharacterText;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.CharacterCurrency] = (Action)Delegate.Combine(onCurrencyChange[Currencies.CharacterCurrency], new Action(ManageAllLevelStatButtons));
		int num = 0;
		for (int i = 0; i < StatsParent.childCount; i++)
		{
			CharacterStatInfo stat = DatabaseManager.CharacterStatList.OrderBy((CharacterStatInfo a) => a.OrderOfAppearance).ToList()[num];
			Transform child = StatsParent.GetChild(i);
			StatTransforms.Add(stat.FunctionName, child);
			Icons.Add(stat.FunctionName, child.Find("IconPlace").Find("Icon").GetComponent<Image>());
			Description.Add(stat.FunctionName, child.Find("DescPlace").Find("DescText").GetComponent<TextMeshProUGUI>());
			Costs.Add(stat.FunctionName, child.Find("PurchaseButton").Find("CostText").GetComponent<TextMeshProUGUI>());
			LevelStatButtons.Add(stat.FunctionName, child.Find("PurchaseButton").GetComponent<Button>());
			LevelStatGOs.Add(stat.FunctionName, child.Find("PurchaseButton").gameObject);
			IconLockedGOs.Add(stat.FunctionName, child.Find("IconPlace_Locked").gameObject);
			IconUnlockedGOs.Add(stat.FunctionName, child.Find("IconPlace").gameObject);
			DescImageLockedGOs.Add(stat.FunctionName, child.Find("DescPlace").Find("Locked").gameObject);
			DescImageUnlockedGOs.Add(stat.FunctionName, child.Find("DescPlace").Find("Unlocked").gameObject);
			LevelTexts.Add(stat.FunctionName, child.Find("LevelText").GetComponent<TextMeshProUGUI>());
			LevelStatButtons[stat.FunctionName].onClick.AddListener(delegate
			{
				ClickedOnLevelStat(stat.FunctionName);
			});
			num++;
		}
		foreach (CharacterStatInfo characterStat in DatabaseManager.CharacterStatList)
		{
			UpdateStat(characterStat.FunctionName);
		}
		ManageAllLevelStatButtons();
		ManageLevelCharacterText();
		UpdateDamageText();
	}

	private void UpdateStat(string statName)
	{
		CharacterStatInfo characterStatInfo = DatabaseManager.CharacterStatDict[statName];
		bool flag = playerData.instance.CharacterLevel < characterStatInfo.UnlockLevel;
		int num = playerData.instance.CharacterStats_Levels[statName];
		double num2 = ExpressionEvaluator.Evaluate(characterStatInfo.ValueEquation, (num <= 0) ? 1 : num);
		Description[statName].text = (flag ? LocalizerManager.GetTranslatedThenReplaceValues("ReachArcherLevelText", characterStatInfo.UnlockLevel.ToString()) : GetDescriptionText(characterStatInfo, (float)num2));
		LevelTexts[statName].text = (flag ? "" : ((num >= characterStatInfo.MaxLevel) ? "" : (num + "/" + characterStatInfo.MaxLevel)));
		Icons[statName].sprite = characterStatInfo.Icon;
		DescImageLockedGOs[statName].SetActive(flag);
		DescImageUnlockedGOs[statName].SetActive(!flag);
		LevelStatGOs[statName].SetActive(!flag);
		IconUnlockedGOs[statName].SetActive(!flag);
		IconLockedGOs[statName].SetActive(flag);
		Description[statName].color = (flag ? DescTextLockedColor : DescTextUnlockedColor);
		if (num >= characterStatInfo.MaxLevel)
		{
			Costs[statName].text = LocalizerManager.GetTranslatedValue("Max_Text");
		}
		else
		{
			Costs[statName].text = "<sprite name=CharacterCurrency>" + ExpressionEvaluator.Evaluate(characterStatInfo.CostEquation, num + 1).ToReadable();
		}
	}

	private string GetDescriptionText(CharacterStatInfo stat, float value)
	{
		if (stat.Stat.VariableName == "Archer_Bounce" || stat.Stat.VariableName == "Archer_Pierce" || stat.Stat.VariableName == "Archer_Chain" || stat.Stat.VariableName == "Archer_AdditionalProjectiles")
		{
			string text = "";
			if (value >= 100f)
			{
				int num = Mathf.FloorToInt(value / 100f);
				value -= (float)(num * 100);
				text = text + LocalizerManager.GetTranslatedThenReplaceValues(stat.Stat.functionName, num.ToString("0")) + "\n";
			}
			if (playerData.instance.CharacterStats_Levels[stat.FunctionName] < stat.MaxLevel)
			{
				text += LocalizerManager.GetTranslatedThenReplaceValues(stat.Stat.functionName + "_Chance", value.ToString("0"));
			}
			return text;
		}
		return stat.Stat.GetValueDescText_SingleOrMultipleValues(new List<double> { value }, isColoredTag: false);
	}

	public void UpdateDamageText()
	{
		double num = playerData.instance.stats.Archer_DamageMultiplier.Total.RealValue;
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerTotalShinyFound.Total.RealValue * (float)playerData.instance.TotalShinyFound / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerStatsInAllItemsEquipped.Total.RealValue * (float)playerData.instance.TotalStatsInItemsEquipped / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerBountyFound.Total.RealValue * (float)playerData.instance.TotalBountiesFound_CurrentRun / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerLevelOfGems.Total.RealValue * (float)playerData.instance.TotalGemsLeveledUp / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerSecondInTimer.Total.RealValue * playerData.instance.stats.Timer.Total.RealValue / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerAreaMarkApplied.Total.RealValue * (float)playerData.instance.TotalAreaMarksApplied_CurrentRun / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerMonster.Total.RealValue * (float)playerData.instance.stats.NumberOfMonsters.Total.RealValue / 100f);
		double number = playerData.instance.stats.Damage.Total.RealValue * num;
		DamageText.text = "<sprite=11>" + number.ToReadable();
	}

	public void CheckNotifications_EndRun()
	{
		foreach (CharacterStatInfo characterStat in DatabaseManager.CharacterStatList)
		{
			if (playerData.instance.CharacterLevel >= characterStat.UnlockLevel && playerData.instance.CharacterStats_Levels[characterStat.FunctionName] == 0)
			{
				double amount = ExpressionEvaluator.Evaluate(characterStat.CostEquation, 1.0);
				if (PlayerManager.instance.IsCanSpendCurrency(Currencies.CharacterCurrency, amount, IsSpendAlso: false))
				{
					MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Heroes, isShow: true);
					return;
				}
			}
		}
		if (PlayerManager.instance.IsCanSpendCurrency(Currencies.CharacterCurrency, DatabaseManager.ArcherLevelCost(playerData.instance.CharacterLevel + 1), IsSpendAlso: false))
		{
			MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Heroes, isShow: true);
		}
	}

	private void ManageAllLevelStatButtons()
	{
		foreach (CharacterStatInfo characterStat in DatabaseManager.CharacterStatList)
		{
			int num = playerData.instance.CharacterStats_Levels[characterStat.FunctionName];
			if (num < characterStat.MaxLevel)
			{
				double amount = ExpressionEvaluator.Evaluate(characterStat.CostEquation, num + 1);
				LevelStatButtons[characterStat.FunctionName].interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.CharacterCurrency, amount, IsSpendAlso: false);
			}
			else
			{
				LevelStatButtons[characterStat.FunctionName].interactable = false;
			}
		}
		LevelCharacterButton.interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.CharacterCurrency, DatabaseManager.ArcherLevelCost(playerData.instance.CharacterLevel + 1), IsSpendAlso: false);
	}

	private void ManageLevelCharacterText()
	{
		LevelCharacterText.text = LocalizerManager.GetTranslatedValue("Lv_Text") + playerData.instance.CharacterLevel + "\n<sprite name=CharacterCurrency>" + DatabaseManager.ArcherLevelCost(playerData.instance.CharacterLevel + 1);
	}

	public void ClickedOnLevelStat(string statName)
	{
		int num = playerData.instance.CharacterStats_Levels[statName];
		if (num < DatabaseManager.CharacterStatDict[statName].MaxLevel)
		{
			double amount = ExpressionEvaluator.Evaluate(DatabaseManager.CharacterStatDict[statName].CostEquation, num + 1);
			if (PlayerManager.instance.IsCanSpendCurrency(Currencies.CharacterCurrency, amount, IsSpendAlso: true))
			{
				FXManager.instance.PlayUIClickSound();
				playerData.instance.CharacterStats_Levels[statName]++;
				UpdateStat(statName);
				ApplyRemoveStat(statName, isApply: true);
				ManageAllLevelStatButtons();
			}
		}
	}

	private bool IsBounceWithouthBefore(string statName)
	{
		if (statName == "Bounce" && playerData.instance.CharacterStats_Levels["Pierce"] == 0 && playerData.instance.CharacterStats_Levels["Chain"] == 0 && playerData.instance.CharacterStats_Levels["AdditionalProjectiles"] == 0)
		{
			return true;
		}
		return false;
	}

	public void ClickedOnLevelCharacter()
	{
		if (!PlayerManager.instance.IsCanSpendCurrency(Currencies.CharacterCurrency, DatabaseManager.ArcherLevelCost(playerData.instance.CharacterLevel + 1), IsSpendAlso: true))
		{
			return;
		}
		playerData.instance.CharacterLevel++;
		ManageAllLevelStatButtons();
		ManageLevelCharacterText();
		playerData.instance.stats.ChangeAStat("Archer_DamageMultiplier", StatsProperties.Additive, 10.0, IsAdd: true);
		UpdateDamageText();
		foreach (CharacterStatInfo characterStat in DatabaseManager.CharacterStatList)
		{
			UpdateStat(characterStat.FunctionName);
		}
	}

	private void ApplyRemoveStat(string statName, bool isApply)
	{
		int num = playerData.instance.CharacterStats_Levels[statName];
		CharacterStatInfo characterStatInfo = DatabaseManager.CharacterStatDict[statName];
		double num2 = ((num > 0) ? ExpressionEvaluator.Evaluate(characterStatInfo.ValueEquation, num - 1) : 0.0);
		double value = ExpressionEvaluator.Evaluate(characterStatInfo.ValueEquation, num) - num2;
		playerData.instance.stats.ChangeAStat(characterStatInfo.Stat.VariableName, characterStatInfo.Stat.StatsProp, value, isApply);
	}
}
public class EnemiesManager : MonoBehaviour
{
	public enum ForceMonsterSpawnType
	{
		Random,
		Shiny,
		TreasureChest,
		Ghost,
		Ore
	}

	public static EnemiesManager instance;

	public Dictionary<int, EnemySelfer> AliveEnemies_Selfers = new Dictionary<int, EnemySelfer>();

	public Dictionary<int, Transform> AliveEnemies_Transform = new Dictionary<int, Transform>();

	public LayerMask EnemyLayer;

	public Transform WellCenterPoint;

	public float PreventRadiusFromWellCenter = 10f;

	[Header("Smart Clustering Settings")]
	[Tooltip("Maximum number of enemies allowed in the spawn area")]
	private int maxNearbyEnemies = 5;

	[Tooltip("Radius to check for nearby enemies when spawning")]
	private float spawnRadius = 180f;

	[Tooltip("Maximum attempts to find a good spawn position before trying a different target")]
	private int maxSpawnAttempts = 10;

	private Coroutine SpawnLevelCo;

	private float TotalSpawnTime_If30Monster = 0.6f;

	private Vector2 MoveEnemiesEvery = new Vector2(3f, 15f);

	private float currentTimerToMoveEnemies = 1E+09f;

	private bool isFinishedSpawning;

	public Transform Trapezoid_TopLeft_Transform;

	public Transform Trapezoid_TopRight_Transform;

	public Transform Trapezoid_BottomLeft_Transform;

	public Transform Trapezoid_BottomRight_Transform;

	private Vector2 Trapezoid_TopLeft;

	private Vector2 Trapezoid_TopRight;

	private Vector2 Trapezoid_BottomLeft;

	private Vector2 Trapezoid_BottomRight;

	[HideInInspector]
	public Vector2 Trapezoid_Center;

	private float SpawnTimer;

	private Dictionary<ShinyRarity, List<int>> ShinyRarityToSkinID = new Dictionary<ShinyRarity, List<int>>();

	private int totalShinyGainAllGame;

	private int iteratorOfBaseSpawnedEnemy;

	private List<(string, int)> EnemiesFromPreviousExitedRun = new List<(string, int)>();

	private List<(string, int)> EnemiesInCurrentRun = new List<(string, int)>();

	public List<(string, string, int)> ForcedSpawnEnemies_FromAzrar = new List<(string, string, int)>();

	private int howManyTriesToAvoidEnemies;

	private Dictionary<string, float> SpecialMonstersChance = new Dictionary<string, float>();

	private Dictionary<string, float> SpecialMonstersPenaltyMultiplier = new Dictionary<string, float>();

	private Dictionary<string, float> SpecialMonstersPenaltyMultiplierMinimum = new Dictionary<string, float>();

	private Dictionary<string, bool> IsSpecialMonsterSpawned = new Dictionary<string, bool>();

	private Dictionary<int, float> PackRadiuses = new Dictionary<int, float>();

	private float maxPackRadius;

	[HideInInspector]
	public double TotalDamageTakenByEnemies;

	private bool isDontSpawn_FinishedGame;

	private float minimumEmptySize = 120f;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		isFinishedSpawning = false;
		Trapezoid_TopLeft = Trapezoid_TopLeft_Transform.position;
		Trapezoid_TopRight = Trapezoid_TopRight_Transform.position;
		Trapezoid_BottomLeft = Trapezoid_BottomLeft_Transform.position;
		Trapezoid_BottomRight = Trapezoid_BottomRight_Transform.position;
		Trapezoid_Center = (Trapezoid_TopLeft + Trapezoid_TopRight + Trapezoid_BottomLeft + Trapezoid_BottomRight) / 4f;
		foreach (ShinyInfo shiny in DatabaseManager.ShinyList)
		{
			if (!ShinyRarityToSkinID.ContainsKey(shiny.Rarity))
			{
				ShinyRarityToSkinID.Add(shiny.Rarity, new List<int>());
			}
			ShinyRarityToSkinID[shiny.Rarity].Add(shiny.SkinID);
		}
		EnemiesFromPreviousExitedRun = new List<(string, int)>();
		EnemiesInCurrentRun = new List<(string, int)>();
		SpecialMonstersChance.Add("TreasureChest", 0f);
		SpecialMonstersChance.Add("Shiny", 0f);
		SpecialMonstersChance.Add("Ghost", 0f);
		SpecialMonstersChance.Add("Ore", 0f);
		SpecialMonstersPenaltyMultiplier.Add("TreasureChest", 0.9f);
		SpecialMonstersPenaltyMultiplier.Add("Shiny", 0.94f);
		SpecialMonstersPenaltyMultiplier.Add("Ghost", 0.93f);
		SpecialMonstersPenaltyMultiplier.Add("Ore", 0.93f);
		SpecialMonstersPenaltyMultiplierMinimum.Add("TreasureChest", 0.45f);
		SpecialMonstersPenaltyMultiplierMinimum.Add("Shiny", 0.6f);
		SpecialMonstersPenaltyMultiplierMinimum.Add("Ghost", 0.45f);
		SpecialMonstersPenaltyMultiplierMinimum.Add("Ore", 0.65f);
		IsSpecialMonsterSpawned.Add("TreasureChest", value: false);
		IsSpecialMonsterSpawned.Add("Shiny", value: false);
		IsSpecialMonsterSpawned.Add("Ghost", value: false);
		IsSpecialMonsterSpawned.Add("Ore", value: false);
		PackRadiuses = new Dictionary<int, float>
		{
			{ 1, 20f },
			{ 2, 40f },
			{ 3, 55f },
			{ 4, 65f },
			{ 5, 82f },
			{ 6, 95f },
			{ 7, 107f }
		};
		maxPackRadius = PackRadiuses.Values.Max();
		isDontSpawn_FinishedGame = false;
	}

	public void StartRun()
	{
		isDontSpawn_FinishedGame = false;
		iteratorOfBaseSpawnedEnemy = 0;
		howManyTriesToAvoidEnemies = 0;
		TotalDamageTakenByEnemies = 0.0;
		EnemySelfer.HowManyTimesHitSoundPlayed = 0;
		SpawnManager();
		SpawnTimer = TotalSpawnTime_If30Monster;
		totalShinyGainAllGame = playerData.instance.ShinyCounts.Values.Sum();
	}

	public void EndRun(bool isExitedRun)
	{
		isFinishedSpawning = false;
		if (isExitedRun)
		{
			if (!RunManager.instance.IsBossRun)
			{
				EnemiesFromPreviousExitedRun = EnemiesInCurrentRun;
			}
			EnemiesInCurrentRun = new List<(string, int)>();
		}
		else
		{
			foreach (string item in IsSpecialMonsterSpawned.Keys.ToList())
			{
				if (!IsSpecialMonsterSpawned[item])
				{
					switch (item)
					{
					case "Shiny":
						playerData.instance.SpecialMonstersUnluckyProtection[item] += playerData.instance.stats.ShinyChance.Total.RealValue + playerData.instance.stats.NonNormalMonsterChance.Total.RealValue;
						break;
					case "Ghost":
						playerData.instance.SpecialMonstersUnluckyProtection[item] += playerData.instance.stats.GhostChance.Total.RealValue + playerData.instance.stats.NonNormalMonsterChance.Total.RealValue;
						break;
					case "Ore":
						playerData.instance.SpecialMonstersUnluckyProtection[item] += playerData.instance.stats.OreChance.Total.RealValue + playerData.instance.stats.NonNormalMonsterChance.Total.RealValue;
						break;
					}
				}
				IsSpecialMonsterSpawned[item] = false;
			}
		}
		foreach (EnemySelfer value in AliveEnemies_Selfers.Values)
		{
			value.ForceStop();
		}
	}

	public void AddAForcedSpawnEnemy_FromAzrar(ForceMonsterSpawnType forceMonsterSpawnType)
	{
		switch (forceMonsterSpawnType)
		{
		case ForceMonsterSpawnType.Shiny:
		{
			for (int j = 0; j < 3; j++)
			{
				int item = 0;
				if (GetShinyRarity(out var rarity))
				{
					item = GetShinySkinID(rarity);
				}
				ForcedSpawnEnemies_FromAzrar.Add(("Slime", "_Normal_Prefab", item));
			}
			break;
		}
		case ForceMonsterSpawnType.Ghost:
			ForcedSpawnEnemies_FromAzrar.Add(("Ghost", "_Boss_Prefab", 5));
			break;
		case ForceMonsterSpawnType.Ore:
		{
			for (int i = 0; i < 3; i++)
			{
				ForcedSpawnEnemies_FromAzrar.Add(("Ore", "_Normal_Prefab", 0));
			}
			break;
		}
		}
	}

	public float PartialEnemiesDead()
	{
		float num = 0f;
		foreach (EnemySelfer value in AliveEnemies_Selfers.Values)
		{
			if (!value.isDead && value.myStats.CurrentHealth < value.myStats.Health.Total.RealValue && value.myStats.CurrentHealth > 0.0)
			{
				num += (float)(value.myStats.CurrentHealth / value.myStats.Health.Total.RealValue);
			}
		}
		return num;
	}

	public void ForceClearEnemiesInPreviousRun()
	{
		EnemiesFromPreviousExitedRun = new List<(string, int)>();
		EnemiesInCurrentRun = new List<(string, int)>();
	}

	public Vector2 SampleRandomPointInTrapezoid(bool isAvoidEnemies)
	{
		float num = Mathf.Min(Trapezoid_TopLeft.y, Trapezoid_TopRight.y, Trapezoid_BottomLeft.y, Trapezoid_BottomRight.y);
		float num2 = Mathf.Max(Trapezoid_TopLeft.y, Trapezoid_TopRight.y, Trapezoid_BottomLeft.y, Trapezoid_BottomRight.y);
		float num3 = UnityEngine.Random.Range(num, num2);
		float t = (num3 - num) / (num2 - num);
		Vector2 vector = Vector2.Lerp(Trapezoid_BottomLeft, Trapezoid_TopLeft, t);
		Vector2 vector2 = Vector2.Lerp(Trapezoid_BottomRight, Trapezoid_TopRight, t);
		float x = UnityEngine.Random.Range(vector.x, vector2.x);
		Vector2 vector3 = new Vector2(x, num3);
		if (WellManager.instance.isWellInRun && Vector2.Distance(vector3, WellCenterPoint.position) < PreventRadiusFromWellCenter)
		{
			return SampleRandomPointInTrapezoid(isAvoidEnemies);
		}
		if (isAvoidEnemies && howManyTriesToAvoidEnemies < 20)
		{
			foreach (EnemySelfer value in AliveEnemies_Selfers.Values)
			{
				if (Vector2.Distance(vector3, value.transform.position) < 50f)
				{
					howManyTriesToAvoidEnemies++;
					return SampleRandomPointInTrapezoid(isAvoidEnemies);
				}
			}
		}
		howManyTriesToAvoidEnemies = 0;
		return vector3;
	}

	public bool IsPointInTrapezoid(Vector2 point)
	{
		if (WellManager.instance.isWellInRun && Vector2.Distance(point, WellCenterPoint.position) < PreventRadiusFromWellCenter)
		{
			return false;
		}
		float num = Mathf.Min(Trapezoid_TopLeft.y, Trapezoid_TopRight.y, Trapezoid_BottomLeft.y, Trapezoid_BottomRight.y);
		float num2 = Mathf.Max(Trapezoid_TopLeft.y, Trapezoid_TopRight.y, Trapezoid_BottomLeft.y, Trapezoid_BottomRight.y);
		if (point.y < num || point.y > num2)
		{
			return false;
		}
		float t = (point.y - num) / (num2 - num);
		Vector2 vector = Vector2.Lerp(Trapezoid_BottomLeft, Trapezoid_TopLeft, t);
		Vector2 vector2 = Vector2.Lerp(Trapezoid_BottomRight, Trapezoid_TopRight, t);
		if (point.x >= vector.x)
		{
			return point.x <= vector2.x;
		}
		return false;
	}

	public List<Vector2> SampleEvenlySpacedPointsInTrapezoid(int numPoints, int candidatesPerPoint = 10)
	{
		List<Vector2> list = new List<Vector2>();
		if (numPoints <= 0)
		{
			return list;
		}
		list.Add(SampleRandomPointInTrapezoid(isAvoidEnemies: false));
		for (int i = 1; i < numPoints; i++)
		{
			Vector2 item = Vector2.zero;
			float num = 0f;
			for (int j = 0; j < candidatesPerPoint; j++)
			{
				Vector2 vector = SampleRandomPointInTrapezoid(isAvoidEnemies: false);
				float num2 = float.MaxValue;
				foreach (Vector2 item2 in list)
				{
					float num3 = Vector2.Distance(vector, item2);
					if (num3 < num2)
					{
						num2 = num3;
					}
				}
				if (num2 > num)
				{
					num = num2;
					item = vector;
				}
			}
			list.Add(item);
		}
		return list;
	}

	public void SpawnManager()
	{
		currentTimerToMoveEnemies = 1E+09f;
		if (SpawnLevelCo != null)
		{
			StopCoroutine(SpawnLevelCo);
		}
		SpawnLevelCo = StartCoroutine(SpawnManager_Co());
	}

	private IEnumerator SpawnManager_Pack_Co()
	{
		int packSize = 4;
		int NumberOfPacks = Mathf.CeilToInt(playerData.instance.stats.NumberOfMonsters.Total.RealValue / packSize);
		if (RunManager.instance.IsBossRun)
		{
			UnityEngine.Debug.Log("NOT IMPLEMENTED");
		}
		float packRadius = PackRadiuses.GetValueOrDefault(packSize, maxPackRadius + (float)(10 * (packSize - 7)));
		List<Vector2> positions2 = SampleEvenlySpacedPointsInTrapezoid(NumberOfPacks, 250);
		positions2 = positions2.OrderBy((Vector2 pos) => pos.x).ToList();
		List<int> list = AliveEnemies_Selfers.Keys.ToList();
		for (int j = 0; j < list.Count; j++)
		{
			AliveEnemies_Selfers[list[j]].StartedDying("", isForce: true);
			EnemyDied(list[j], isForceRemove: true);
		}
		for (int i = 0; i < positions2.Count; i++)
		{
			List<Vector2> list2 = FunctionsNeeded.SampleEvenlySpacedPointsInCircle(packSize, positions2[i], packRadius);
			foreach (Vector2 item in list2)
			{
				CreateAnEnemy(isStart: true, item);
				iteratorOfBaseSpawnedEnemy++;
				yield return new WaitForSeconds(TotalSpawnTime_If30Monster / ((float)NumberOfPacks * (float)packSize));
			}
		}
		yield return null;
		currentTimerToMoveEnemies = 1f;
		isFinishedSpawning = true;
	}

	private IEnumerator SpawnManager_Co()
	{
		int numberOfEnemies = playerData.instance.stats.NumberOfMonsters.Total.RealValue;
		if (RunManager.instance.IsBossRun)
		{
			numberOfEnemies = DatabaseManager.BaseNumberOfMonstersInBossRun;
		}
		numberOfEnemies = Mathf.Min(numberOfEnemies, 60);
		List<Vector2> positions2 = SampleEvenlySpacedPointsInTrapezoid(numberOfEnemies, 5);
		if (positions2.Count > 1)
		{
			int num = Mathf.Clamp(playerData.instance.MonstersLevel, 1, 5);
			for (int j = 0; j < num; j++)
			{
				int index = UnityEngine.Random.Range(0, positions2.Count - num);
				Vector2 vector = positions2[index];
				Vector2 vector3;
				do
				{
					Vector2 vector2 = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(90, 130);
					vector3 = vector + vector2;
				}
				while (!IsPointInTrapezoid(vector3));
				positions2[positions2.Count - 1 - j] = vector3;
			}
		}
		positions2 = positions2.OrderBy((Vector2 pos) => pos.x).ToList();
		List<int> list = AliveEnemies_Selfers.Keys.ToList();
		for (int k = 0; k < list.Count; k++)
		{
			AliveEnemies_Selfers[list[k]].StartedDying("", isForce: true);
			EnemyDied(list[k], isForceRemove: true);
		}
		for (int i = 0; i < numberOfEnemies; i++)
		{
			CreateAnEnemy(isStart: true, positions2[i]);
			iteratorOfBaseSpawnedEnemy++;
			yield return new WaitForSeconds(TotalSpawnTime_If30Monster / (float)numberOfEnemies);
		}
		yield return null;
		currentTimerToMoveEnemies = 1f;
		isFinishedSpawning = true;
	}

	public void SpawnASingleMonster(ForceMonsterSpawnType forceMonsterSpawnType = ForceMonsterSpawnType.Random, bool isForceRandomSpawn = false)
	{
		if (!FunctionsNeeded.IsHappened(playerData.instance.stats.MonstersSpawnInClusters.Total.RealValue) || isForceRandomSpawn)
		{
			Vector2 vector = SampleRandomPointInTrapezoid(isAvoidEnemies: false);
			CreateAnEnemy(isStart: false, vector, forceMonsterSpawnType);
			return;
		}
		if (AliveEnemies_Transform.Count == 0)
		{
			Vector2 vector2 = SampleRandomPointInTrapezoid(isAvoidEnemies: false);
			CreateAnEnemy(isStart: false, vector2, forceMonsterSpawnType);
			return;
		}
		int num = 0;
		List<Transform> list = AliveEnemies_Transform.Values.ToList();
		Vector2 vector3;
		do
		{
			vector3 = list.GetOneRandom().position;
			num++;
		}
		while (CountNearbyEnemies(vector3, spawnRadius) > maxNearbyEnemies && num < maxSpawnAttempts);
		if (num >= maxSpawnAttempts)
		{
			Vector2 vector4 = SampleRandomPointInTrapezoid(isAvoidEnemies: false);
			CreateAnEnemy(isStart: false, vector4, forceMonsterSpawnType);
			return;
		}
		Vector2 vector6;
		do
		{
			Vector2 vector5 = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(80, 110);
			vector6 = vector3 + vector5;
		}
		while (!IsPointInTrapezoid(vector6));
		CreateAnEnemy(isStart: false, vector6, forceMonsterSpawnType);
	}

	private int CountNearbyEnemies(Vector2 position, float radius)
	{
		int num = 0;
		foreach (KeyValuePair<int, Transform> item in AliveEnemies_Transform)
		{
			if (Vector2.Distance(position, item.Value.position) <= radius)
			{
				num++;
			}
		}
		return num;
	}

	public void SpawnPack(ForceMonsterSpawnType forceMonsterSpawnType = ForceMonsterSpawnType.Random)
	{
		int num = 4;
		if (playerData.instance.stats.NumberOfMonsters.Total.RealValue - AliveEnemies_Selfers.Count < num)
		{
			return;
		}
		float valueOrDefault = PackRadiuses.GetValueOrDefault(num, maxPackRadius + (float)(10 * (num - 7)));
		Vector2 centerOfPack = GetCenterOfPack(valueOrDefault);
		foreach (Vector2 item in FunctionsNeeded.SampleEvenlySpacedPointsInCircle(num, centerOfPack, valueOrDefault, 50))
		{
			CreateAnEnemy(isStart: false, item, forceMonsterSpawnType);
		}
	}

	private Vector2 GetCenterOfPack(float packRadius)
	{
		new Vector2(0f - packRadius, 0f);
		new Vector2(packRadius, 0f);
		new Vector2(0f, packRadius);
		new Vector2(0f, 0f - packRadius);
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < 100; i++)
		{
			Vector2 item = SampleRandomPointInTrapezoid(isAvoidEnemies: false);
			list.Add(item);
		}
		List<Vector2> list2 = AliveEnemies_Transform.Values.Select((Transform x) => new Vector2(x.position.x, x.position.y)).ToList();
		Vector2 result = list[0];
		float num = float.MinValue;
		foreach (Vector2 item2 in list)
		{
			float num2 = float.MaxValue;
			foreach (Vector2 item3 in list2)
			{
				float num3 = Vector2.Distance(item2, item3);
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			if (num2 > num)
			{
				num = num2;
				result = item2;
			}
		}
		return result;
	}

	public void CreateAnEnemy(bool isStart, Vector3 Pos, ForceMonsterSpawnType forceMonsterSpawnType = ForceMonsterSpawnType.Random)
	{
		if (isDontSpawn_FinishedGame || (!RunManager.instance.IsBossRun && forceMonsterSpawnType == ForceMonsterSpawnType.Random && AliveEnemies_Selfers.Count >= playerData.instance.stats.NumberOfMonsters.Total.RealValue) || (RunManager.instance.IsBossRun && AliveEnemies_Selfers.Count >= DatabaseManager.BaseNumberOfMonstersInBossRun))
		{
			return;
		}
		int monstersLevel = playerData.instance.MonstersLevel;
		if (FunctionsNeeded.IsHappened((monstersLevel < 5) ? 70 : ((monstersLevel < 20) ? 25 : ((monstersLevel < 30) ? 15 : 7))))
		{
			FXManager.instance.PlaySound("EnemySpawnSound");
		}
		string text = "Slime";
		int num = 0;
		string text2 = "_Normal_Prefab";
		if (!RunManager.instance.IsBossRun && !isStart && ForcedSpawnEnemies_FromAzrar.Count > 0)
		{
			(string, string, int) oneRandom = ForcedSpawnEnemies_FromAzrar.GetOneRandom();
			(text, text2, num) = oneRandom;
			ForcedSpawnEnemies_FromAzrar.Remove(oneRandom);
		}
		else if (EnemiesFromPreviousExitedRun.Count > 0)
		{
			(string, int) oneRandom2 = EnemiesFromPreviousExitedRun.GetOneRandom();
			(text, num) = oneRandom2;
			EnemiesFromPreviousExitedRun.Remove(oneRandom2);
		}
		else
		{
			float realValue = playerData.instance.stats.NonNormalMonsterChance.Total.RealValue;
			SpecialMonstersChance["TreasureChest"] = (playerData.instance.stats.TreasureChance.Total.RealValue + realValue) * ((playerData.instance.MonstersLevel >= DatabaseManager.MaxMonstersLevelInGame) ? 1f : Mathf.Max(playerData.instance.SpecialMonstersPenalty["TreasureChest"], SpecialMonstersPenaltyMultiplierMinimum["TreasureChest"]));
			SpecialMonstersChance["Shiny"] = (playerData.instance.stats.ShinyChance.Total.RealValue + realValue) * ((playerData.instance.MonstersLevel >= DatabaseManager.MaxMonstersLevelInGame) ? 1f : Mathf.Max(playerData.instance.SpecialMonstersPenalty["Shiny"], SpecialMonstersPenaltyMultiplierMinimum["Shiny"]));
			SpecialMonstersChance["Ghost"] = (playerData.instance.stats.GhostChance.Total.RealValue + realValue) * ((playerData.instance.MonstersLevel >= DatabaseManager.MaxMonstersLevelInGame) ? 1f : Mathf.Max(playerData.instance.SpecialMonstersPenalty["Ghost"], SpecialMonstersPenaltyMultiplierMinimum["Ghost"]));
			SpecialMonstersChance["Ore"] = (playerData.instance.stats.OreChance.Total.RealValue + realValue) * ((playerData.instance.MonstersLevel >= DatabaseManager.MaxMonstersLevelInGame) ? 1f : Mathf.Max(playerData.instance.SpecialMonstersPenalty["Ore"], SpecialMonstersPenaltyMultiplierMinimum["Ore"]));
			if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Shiny] && iteratorOfBaseSpawnedEnemy >= 10)
			{
				playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Shiny] = false;
				num = DatabaseManager.ShinyList.Find((ShinyInfo x) => x.OrderOfAppearance == 0).SkinID;
			}
			else if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.TreasureChest] && iteratorOfBaseSpawnedEnemy >= 10)
			{
				playerData.instance.IsJustUnlockedSystem[UnlockableSystems.TreasureChest] = false;
				text = "TreasureChest";
			}
			else if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Heroes] && iteratorOfBaseSpawnedEnemy >= 10)
			{
				playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Heroes] = false;
				text = "Ghost";
				num = 5;
			}
			else if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Gems] && iteratorOfBaseSpawnedEnemy >= 10)
			{
				playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Gems] = false;
				text = "Ore";
				num = 0;
			}
			else if (playerData.instance.isJustUnlockedRareShiny && iteratorOfBaseSpawnedEnemy >= 10)
			{
				playerData.instance.isJustUnlockedRareShiny = false;
				num = DatabaseManager.ShinyDict["Lightning"].SkinID;
			}
			else if (playerData.instance.isJustUnlockedEpicShiny && iteratorOfBaseSpawnedEnemy >= 10)
			{
				playerData.instance.isJustUnlockedEpicShiny = false;
				num = DatabaseManager.ShinyDict["Angel"].SkinID;
			}
			else if (forceMonsterSpawnType == ForceMonsterSpawnType.Shiny || (playerData.instance.UnlockedSystems[UnlockableSystems.Shiny] && FunctionsNeeded.IsHappened(SpecialMonstersChance["Shiny"])))
			{
				if (GetShinyRarity(out var rarity))
				{
					num = GetShinySkinID(rarity);
				}
				if (num > 0)
				{
					playerData.instance.SpecialMonstersPenalty["Shiny"] *= SpecialMonstersPenaltyMultiplier["Shiny"];
					IsSpecialMonsterSpawned["Shiny"] = true;
				}
			}
			else if (forceMonsterSpawnType == ForceMonsterSpawnType.TreasureChest || (playerData.instance.UnlockedSystems[UnlockableSystems.TreasureChest] && FunctionsNeeded.IsHappened(SpecialMonstersChance["TreasureChest"])))
			{
				text = "TreasureChest";
				playerData.instance.SpecialMonstersPenalty["TreasureChest"] *= SpecialMonstersPenaltyMultiplier["TreasureChest"];
				IsSpecialMonsterSpawned["TreasureChest"] = true;
			}
			else if (forceMonsterSpawnType == ForceMonsterSpawnType.Ghost || (playerData.instance.UnlockedSystems[UnlockableSystems.Heroes] && FunctionsNeeded.IsHappened(SpecialMonstersChance["Ghost"])))
			{
				text = "Ghost";
				num = 5;
				playerData.instance.SpecialMonstersPenalty["Ghost"] *= SpecialMonstersPenaltyMultiplier["Ghost"];
				IsSpecialMonsterSpawned["Ghost"] = true;
			}
			else if (forceMonsterSpawnType == ForceMonsterSpawnType.Ore || (playerData.instance.UnlockedSystems[UnlockableSystems.Gems] && FunctionsNeeded.IsHappened(SpecialMonstersChance["Ore"])))
			{
				text = "Ore";
				num = (FunctionsNeeded.IsHappened(playerData.instance.stats.OreChance_Rich.Total.RealValue) ? 1 : 0);
				playerData.instance.SpecialMonstersPenalty["Ore"] *= SpecialMonstersPenaltyMultiplier["Ore"];
				IsSpecialMonsterSpawned["Ore"] = true;
			}
		}
		if (RunManager.instance.IsBossRun)
		{
			text = "Slime";
			num = 0;
			if (FunctionsNeeded.IsHappened(30f))
			{
				num = UnityEngine.Random.Range(0, 11);
			}
			text2 = "_Boss_Prefab";
		}
		EnemiesInCurrentRun.Add((text, num));
		GameObject gameObject = ObjectPooler.instance.GiveMeObject(text + text2, base.transform, Pos);
		EnemySelfer component = gameObject.GetComponent<EnemySelfer>();
		component.TakeInfo();
		component.ChangeSkin(num);
		int hashCode = gameObject.GetHashCode();
		gameObject.name = "Enemy__" + hashCode;
		if (!AliveEnemies_Selfers.ContainsKey(hashCode))
		{
			AliveEnemies_Selfers.Add(hashCode, component);
			AliveEnemies_Transform.Add(hashCode, gameObject.transform);
		}
	}

	private bool GetShinyRarity(out ShinyRarity rarity)
	{
		Dictionary<ShinyRarity, float> dictionary = new Dictionary<ShinyRarity, float>();
		dictionary.Add(ShinyRarity.Normal, DatabaseManager.ShinyRarityChanceToDrop[ShinyRarity.Normal]);
		if (playerData.instance.stats.ShinyRare_CanBeFoundInRun.Total.RealValue > 1)
		{
			dictionary.Add(ShinyRarity.Rare, DatabaseManager.ShinyRarityChanceToDrop[ShinyRarity.Rare]);
		}
		if (playerData.instance.stats.ShinyEpic_CanBeFoundInRun.Total.RealValue > 1)
		{
			dictionary.Add(ShinyRarity.Epic, DatabaseManager.ShinyRarityChanceToDrop[ShinyRarity.Epic]);
		}
		rarity = FunctionsNeeded.GetARandomFromDict_RarityIncrease(dictionary, playerData.instance.stats.ShinyRarity.Total.RealValue);
		if (playerData.instance.stats.Well_ShinyCanBeChosenToFarm.Total.RealValue > 50 && playerData.instance.SelectedShiny != "")
		{
			ShinyInfo shinyInfo = DatabaseManager.ShinyDict[playerData.instance.SelectedShiny];
			if (shinyInfo.Rarity != rarity && ((shinyInfo.Rarity == ShinyRarity.Rare && rarity == ShinyRarity.Normal) || shinyInfo.Rarity == ShinyRarity.Epic))
			{
				rarity = FunctionsNeeded.GetARandomFromDict_RarityIncrease(dictionary, playerData.instance.stats.ShinyRarity.Total.RealValue);
			}
		}
		if (IsAllShinyOfThisRarityMaxLevel(rarity))
		{
			return false;
		}
		return true;
	}

	private int GetShinySkinID(ShinyRarity rarity)
	{
		int num = ShinyRarityToSkinID[rarity].Where((int x) => playerData.instance.ShinyCounts[DatabaseManager.ShinyDict_SkinID[x].FunctionName] < DatabaseManager.ShinyDict_SkinID[x].MaxLevel).ToList().GetOneRandom();
		if (playerData.instance.stats.Well_ShinyCanBeChosenToFarm.Total.RealValue > 50 && playerData.instance.SelectedShiny != "")
		{
			ShinyInfo shinyInfo = DatabaseManager.ShinyDict[playerData.instance.SelectedShiny];
			if (shinyInfo.Rarity == rarity && shinyInfo.SkinID != num)
			{
				num = ShinyRarityToSkinID[rarity].Where((int x) => playerData.instance.ShinyCounts[DatabaseManager.ShinyDict_SkinID[x].FunctionName] < DatabaseManager.ShinyDict_SkinID[x].MaxLevel).ToList().GetOneRandom();
			}
		}
		if (!playerData.instance.UnlockedSystems[UnlockableSystems.Towers] && num == DatabaseManager.ShinyDict["Fire"].SkinID)
		{
			num = (FunctionsNeeded.IsHappened(50f) ? DatabaseManager.ShinyDict["Bird"].SkinID : DatabaseManager.ShinyDict["Lightning"].SkinID);
		}
		if (playerData.instance.ShinyCounts["Grass"] < 20 && rarity == ShinyRarity.Normal && num != DatabaseManager.ShinyDict["Grass"].SkinID)
		{
			num = ShinyRarityToSkinID[rarity].Where((int x) => playerData.instance.ShinyCounts[DatabaseManager.ShinyDict_SkinID[x].FunctionName] < DatabaseManager.ShinyDict_SkinID[x].MaxLevel).ToList().GetOneRandom();
		}
		if (playerData.instance.MonstersLevel < 23 && num == DatabaseManager.ShinyDict["Evil"].SkinID)
		{
			num = DatabaseManager.ShinyDict["Angel"].SkinID;
		}
		return num;
	}

	private bool IsAllShinyOfThisRarityMaxLevel(ShinyRarity rarity)
	{
		foreach (KeyValuePair<int, ShinyInfo> item in DatabaseManager.ShinyDict_SkinID)
		{
			if (item.Value.Rarity == rarity && playerData.instance.ShinyCounts[item.Value.FunctionName] < item.Value.MaxLevel)
			{
				return false;
			}
		}
		return true;
	}

	private void DetermineEnemyMoving()
	{
		currentTimerToMoveEnemies -= Time.deltaTime;
		if (currentTimerToMoveEnemies <= 0f)
		{
			currentTimerToMoveEnemies = UnityEngine.Random.Range(MoveEnemiesEvery.x, MoveEnemiesEvery.y);
			MoveEnemiesLogic();
		}
	}

	private bool DoesLineCrossCircle(Vector2 lineStart, Vector2 lineEnd, Vector2 circleCenter, float circleRadius)
	{
		Vector2 vector = lineEnd - lineStart;
		float magnitude = vector.magnitude;
		Vector2 vector2 = vector / magnitude;
		float value = Vector2.Dot(circleCenter - lineStart, vector2);
		value = Mathf.Clamp(value, 0f, magnitude);
		return Vector2.Distance(lineStart + vector2 * value, circleCenter) < circleRadius;
	}

	private void MoveEnemiesLogic()
	{
		int num = UnityEngine.Random.Range(1, 6);
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			EnemySelfer randomEnemy = GetRandomEnemy();
			if (randomEnemy == null || randomEnemy.isMoving || randomEnemy.isDead)
			{
				continue;
			}
			Vector2 vector = SampleRandomPointInTrapezoid(isAvoidEnemies: false);
			if (WellManager.instance.isWellInRun)
			{
				Vector2 lineStart = randomEnemy.transform.position;
				int num3 = 10;
				int num4 = 0;
				while (DoesLineCrossCircle(lineStart, vector, WellCenterPoint.position, PreventRadiusFromWellCenter) && num4 < num3)
				{
					vector = SampleRandomPointInTrapezoid(isAvoidEnemies: false);
					num4++;
				}
				if (DoesLineCrossCircle(lineStart, vector, WellCenterPoint.position, PreventRadiusFromWellCenter))
				{
					continue;
				}
			}
			randomEnemy.StartCoroutine(randomEnemy.MoveStopEnemy(isMove: true, vector, num2));
			num2 += UnityEngine.Random.Range(0.9f, 2.5f);
		}
	}

	public EnemySelfer GetLowestHealthEnemy(bool isSupport = false)
	{
		double num = double.MaxValue;
		int key = 0;
		bool flag = false;
		foreach (KeyValuePair<int, EnemySelfer> aliveEnemies_Selfer in AliveEnemies_Selfers)
		{
			double num2 = aliveEnemies_Selfer.Value.myStats.CurrentHealth / aliveEnemies_Selfer.Value.myStats.Health.Total.RealValue;
			if (num2 < num)
			{
				flag = true;
				key = aliveEnemies_Selfer.Key;
				num = num2;
			}
		}
		if (flag)
		{
			return AliveEnemies_Selfers[key];
		}
		return null;
	}

	public Vector2 GetLocationWithHighestMonsterDensity(float radius)
	{
		if (AliveEnemies_Transform == null || AliveEnemies_Transform.Count == 0)
		{
			return Trapezoid_Center;
		}
		int num = 0;
		Vector2 result = Trapezoid_Center;
		foreach (KeyValuePair<int, Transform> item in AliveEnemies_Transform)
		{
			Vector2 vector = item.Value.position;
			int num2 = 0;
			foreach (KeyValuePair<int, Transform> item2 in AliveEnemies_Transform)
			{
				if (item.Key != item2.Key && Vector2.Distance(vector, item2.Value.position) <= radius)
				{
					num2++;
				}
			}
			if (num2 > num)
			{
				num = num2;
				result = vector;
			}
		}
		return result;
	}

	public EnemySelfer GetNearestEnemy(Vector2 Position, float MaxDistance, List<int> ExcludeEnemy = null, bool IsHardExcludeConstrain = false)
	{
		EnemySelfer nearestEnemy = GetNearestEnemy(Position, ExcludeEnemy, IsHardExcludeConstrain);
		if (nearestEnemy == null || Vector2.Distance(Position, nearestEnemy.GetPosition()) > MaxDistance)
		{
			return null;
		}
		return nearestEnemy;
	}

	public EnemySelfer GetNearestEnemy(Vector2 Position, List<int> ExcludeEnemy = null, bool IsHardExcludeConstrain = false)
	{
		if (ExcludeEnemy == null)
		{
			ExcludeEnemy = new List<int>();
		}
		float num = 10000f;
		int key = 0;
		bool flag = false;
		foreach (KeyValuePair<int, Transform> item in AliveEnemies_Transform)
		{
			if (Vector2.Distance(Position, item.Value.position) < num && !ExcludeEnemy.Contains(item.Key))
			{
				flag = true;
				key = item.Key;
				num = Vector2.Distance(Position, item.Value.position);
			}
		}
		if (!flag && AliveEnemies_Selfers.Count > 1 && !IsHardExcludeConstrain)
		{
			foreach (KeyValuePair<int, Transform> item2 in AliveEnemies_Transform)
			{
				if (Vector2.Distance(Position, item2.Value.position) < num && ExcludeEnemy[ExcludeEnemy.Count - 1] != item2.Key)
				{
					flag = true;
					key = item2.Key;
					num = Vector2.Distance(Position, item2.Value.position);
				}
			}
		}
		if (flag)
		{
			return AliveEnemies_Selfers[key];
		}
		return null;
	}

	public EnemySelfer GetFarthestEnemy(Vector2 Position, List<int> ExcludeEnemy = null, bool IsHardExcludeConstrain = false)
	{
		if (ExcludeEnemy == null)
		{
			ExcludeEnemy = new List<int>();
		}
		float num = -1000f;
		int key = 0;
		bool flag = false;
		foreach (KeyValuePair<int, Transform> item in AliveEnemies_Transform)
		{
			if (Vector2.Distance(Position, item.Value.position) > num && !ExcludeEnemy.Contains(item.Key))
			{
				flag = true;
				key = item.Key;
				num = Vector2.Distance(Position, item.Value.position);
			}
		}
		if (!flag && AliveEnemies_Selfers.Count > 1 && !IsHardExcludeConstrain)
		{
			foreach (KeyValuePair<int, Transform> item2 in AliveEnemies_Transform)
			{
				if (Vector2.Distance(Position, item2.Value.position) > num && ExcludeEnemy[ExcludeEnemy.Count - 1] != item2.Key)
				{
					flag = true;
					key = item2.Key;
					num = Vector2.Distance(Position, item2.Value.position);
				}
			}
		}
		if (!flag && AliveEnemies_Selfers.Count >= 1 && !IsHardExcludeConstrain)
		{
			foreach (KeyValuePair<int, Transform> item3 in AliveEnemies_Transform)
			{
				if (Vector2.Distance(Position, item3.Value.position) > num)
				{
					flag = true;
					key = item3.Key;
					num = Vector2.Distance(Position, item3.Value.position);
				}
			}
		}
		if (flag)
		{
			return AliveEnemies_Selfers[key];
		}
		return null;
	}

	public List<EnemySelfer> GetFarthestEnemies(Vector2 Position, int HowMany, List<int> ExcludeEnemy = null, bool IsHardExcludeConstrain = false)
	{
		if (ExcludeEnemy == null)
		{
			ExcludeEnemy = new List<int>();
		}
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		List<EnemySelfer> list = new List<EnemySelfer>();
		bool flag = false;
		foreach (KeyValuePair<int, Transform> item in AliveEnemies_Transform)
		{
			if (!ExcludeEnemy.Contains(item.Key))
			{
				flag = true;
				dictionary.Add(item.Key, Vector2.Distance(Position, item.Value.position));
			}
		}
		if (!flag && AliveEnemies_Selfers.Count > 1 && !IsHardExcludeConstrain)
		{
			foreach (KeyValuePair<int, Transform> item2 in AliveEnemies_Transform)
			{
				if (ExcludeEnemy[ExcludeEnemy.Count - 1] != item2.Key)
				{
					flag = true;
					dictionary.Add(item2.Key, Vector2.Distance(Position, item2.Value.position));
				}
			}
		}
		if (flag)
		{
			List<int> list2 = (from x in dictionary
				orderby x.Value descending
				select x.Key).ToList();
			int num = ((list2.Count > HowMany) ? HowMany : list2.Count);
			for (int i = 0; i < num; i++)
			{
				list.Add(AliveEnemies_Selfers[list2[i]]);
			}
			return list;
		}
		return null;
	}

	public List<EnemySelfer> GetNearestEnemies(Vector2 Position, int HowMany, List<int> ExcludeEnemy = null, bool IsHardExcludeConstrain = false)
	{
		if (ExcludeEnemy == null)
		{
			ExcludeEnemy = new List<int>();
		}
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		List<EnemySelfer> list = new List<EnemySelfer>();
		bool flag = false;
		foreach (KeyValuePair<int, Transform> item in AliveEnemies_Transform)
		{
			if (!ExcludeEnemy.Contains(item.Key))
			{
				flag = true;
				dictionary.Add(item.Key, Vector2.Distance(Position, item.Value.position));
			}
		}
		if (!flag && AliveEnemies_Selfers.Count > 1 && !IsHardExcludeConstrain)
		{
			foreach (KeyValuePair<int, Transform> item2 in AliveEnemies_Transform)
			{
				if (ExcludeEnemy[ExcludeEnemy.Count - 1] != item2.Key)
				{
					flag = true;
					dictionary.Add(item2.Key, Vector2.Distance(Position, item2.Value.position));
				}
			}
		}
		if (flag)
		{
			List<int> list2 = (from x in dictionary
				orderby x.Value
				select x.Key).ToList();
			int num = ((list2.Count > HowMany) ? HowMany : list2.Count);
			for (int i = 0; i < num; i++)
			{
				list.Add(AliveEnemies_Selfers[list2[i]]);
			}
			return list;
		}
		return null;
	}

	public Dictionary<int, EnemySelfer> GetEnemyNearestToCharacter_PenalizesYDistannce(int HowMany, Vector2 CharacterPos)
	{
		if (AliveEnemies_Transform == null || AliveEnemies_Transform.Count == 0)
		{
			return null;
		}
		float num = 2f;
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		foreach (KeyValuePair<int, Transform> item in AliveEnemies_Transform)
		{
			Vector2 vector = item.Value.position;
			float num2 = Mathf.Abs(vector.x - CharacterPos.x);
			float num3 = Mathf.Abs(vector.y - CharacterPos.y);
			float value = num2 + num3 * num;
			dictionary.Add(item.Key, value);
		}
		Dictionary<int, EnemySelfer> dictionary2 = new Dictionary<int, EnemySelfer>();
		List<int> list = (from x in dictionary
			orderby x.Value
			select x.Key).ToList();
		int num4 = Mathf.Min(HowMany, list.Count);
		for (int i = 0; i < num4; i++)
		{
			int key = list[i];
			dictionary2.Add(key, AliveEnemies_Selfers[key]);
		}
		return dictionary2;
	}

	public Dictionary<int, EnemySelfer> GetEnemiesWithLowstXPos(int HowMany, List<int> ExcludeEnemy = null)
	{
		if (AliveEnemies_Selfers == null || AliveEnemies_Selfers.Count == 0 || HowMany <= 0)
		{
			return new Dictionary<int, EnemySelfer>();
		}
		if (ExcludeEnemy == null)
		{
			ExcludeEnemy = new List<int>();
		}
		return (from enemy in AliveEnemies_Selfers
			where !ExcludeEnemy.Contains(enemy.Key)
			orderby enemy.Value.transform.position.x
			select enemy).Take(HowMany).ToDictionary((KeyValuePair<int, EnemySelfer> enemy) => enemy.Key, (KeyValuePair<int, EnemySelfer> enemy) => enemy.Value);
	}

	public List<T> GetEnemiesInCircle<T>(Vector2 CircleCenter, float Radius, int HowMany = -1)
	{
		List<T> list = new List<T>();
		if (HowMany < 0)
		{
			HowMany = 1000000;
		}
		List<Collider2D> list2 = Physics2D.OverlapCircleAll(CircleCenter, Radius, EnemyLayer).ToList();
		int num = ((list2.Count > HowMany) ? HowMany : list2.Count);
		for (int i = 0; i < num; i++)
		{
			list.Add(list2[i].GetComponent<T>());
		}
		return list;
	}

	public List<EnemySelfer> GetChainedEnemies(Vector2 Pos, int HowMany, float Distance, List<int> ExcludeEnemy)
	{
		List<EnemySelfer> list = new List<EnemySelfer>();
		for (int i = 0; i < HowMany; i++)
		{
			EnemySelfer nearestEnemy = GetNearestEnemy(Pos, ExcludeEnemy, IsHardExcludeConstrain: true);
			if (nearestEnemy == null || Vector2.Distance(Pos, new Vector2(nearestEnemy.transform.position.x, nearestEnemy.transform.position.y)) > Distance)
			{
				break;
			}
			Pos = nearestEnemy.transform.position;
			list.Add(nearestEnemy);
			ExcludeEnemy.Add(nearestEnemy.EnemyHashcode);
		}
		return list;
	}

	public EnemySelfer GetRandomEnemy(List<int> ExcludeEnemy = null)
	{
		if (ExcludeEnemy == null)
		{
			ExcludeEnemy = new List<int>();
		}
		if (AliveEnemies_Selfers.Count > 0)
		{
			List<EnemySelfer> list = AliveEnemies_Selfers.Values.Where((EnemySelfer enemy) => !ExcludeEnemy.Contains(enemy.EnemyHashcode)).ToList();
			if (list.Count > 0)
			{
				return list.GetOneRandom();
			}
			return null;
		}
		return null;
	}

	public List<EnemySelfer> GetRandomEnemies(int HowMany, int ExcludeEnemy)
	{
		List<EnemySelfer> list = new List<EnemySelfer>();
		List<int> list2 = AliveEnemies_Selfers.Keys.ToList();
		list2.Shuffle();
		int num = 0;
		foreach (int item in list2)
		{
			if (item != ExcludeEnemy)
			{
				list.Add(AliveEnemies_Selfers[item]);
			}
			num++;
			if (num >= HowMany)
			{
				break;
			}
		}
		return list;
	}

	public int NumberOfAliveEnemies()
	{
		return AliveEnemies_Selfers.Count;
	}

	public void EnemyDied(int TheHashCode, bool isForceRemove = false)
	{
		if (!isForceRemove && AliveEnemies_Selfers.ContainsKey(TheHashCode) && EnemiesInCurrentRun.Contains((AliveEnemies_Selfers[TheHashCode].enemyType.ToString(), AliveEnemies_Selfers[TheHashCode].SkinID)))
		{
			EnemiesInCurrentRun.Remove((AliveEnemies_Selfers[TheHashCode].enemyType.ToString(), AliveEnemies_Selfers[TheHashCode].SkinID));
		}
		AliveEnemies_Selfers.Remove(TheHashCode);
		AliveEnemies_Transform.Remove(TheHashCode);
		if (!isForceRemove && ((AliveEnemies_Selfers.Count <= 10 && playerData.instance.MonstersLevel > 2) || AliveEnemies_Selfers.Count <= playerData.instance.stats.Well_MonstersSpawnInstantlyIfLessThanX.Total.RealValue))
		{
			for (int i = 0; i < playerData.instance.stats.NumberOfMonstersSpawned.Total.RealValue; i++)
			{
				SpawnASingleMonster(ForceMonsterSpawnType.Random, isForceRandomSpawn: true);
			}
		}
	}

	public void DeSpawnAllEnemies()
	{
		foreach (EnemySelfer item in AliveEnemies_Selfers.Values.ToList())
		{
			item.StopAllCoroutines();
			item.DeathAnimationFinished(isForce: true, isDestroy: true);
		}
		AliveEnemies_Transform.Clear();
		AliveEnemies_Selfers.Clear();
	}

	public void KillAllEnemies_ForFinishGame()
	{
		isDontSpawn_FinishedGame = true;
		foreach (EnemySelfer item in AliveEnemies_Selfers.Values.ToList())
		{
			item.TakeDamage(new DamageData(9.99E+17, IsCrit: false, ""), isOriginalHit: false, isMouseAttack: false, isForceSound: true);
		}
	}

	private void Update()
	{
		if (!isFinishedSpawning || !RunManager.instance.isRunStarted)
		{
			return;
		}
		DetermineEnemyMoving();
		SpawnTimer += Time.deltaTime;
		if ((double)playerData.instance.stats.MonstersSpawnRate.Total.RealValue > 0.01 && SpawnTimer >= 1f / playerData.instance.stats.MonstersSpawnRate.Total.RealValue)
		{
			SpawnTimer = 0f;
			for (int i = 0; i < playerData.instance.stats.NumberOfMonstersSpawned.Total.RealValue; i++)
			{
				SpawnASingleMonster();
			}
		}
	}
}
[CreateAssetMenu]
public class EnemyInfo : ScriptableObject
{
	public GameObject prefab;

	public EnemyType enemyType;

	public int skinNumber;

	public bool isCanAppearFirstFloor = true;

	public Vector2 ShadowPosition_Normal;

	public Vector2 ShadowPosition_Boss;

	public Sprite NormalSprite;

	public Sprite BossSprite;

	[HideInInspector]
	public string functionName => enemyType.ToString() + "_" + skinNumber;
}
public enum EnemyType
{
	Slime,
	Ghost,
	TreasureChest,
	Ore
}
public class EnemySelfer : MonoBehaviour
{
	public EnemyType enemyType;

	[HideInInspector]
	public EnemyStatsData myStats;

	private Rigidbody2D MyRB;

	private Transform MyGFXTrans;

	private Transform MyCenterTrans;

	private Transform MyGroundTrans;

	private Collider2D MyCol;

	[HideInInspector]
	public bool isDead;

	[HideInInspector]
	public int EnemyHashcode;

	public BarSelfer MyHealthBar;

	private GameObject EnemyCanvasGO;

	private bool isInit;

	private SkeletonAnimation monsterAnimator;

	private MeshRenderer monsterRenderer;

	private SpriteRenderer oreRenderer;

	private Vector3 baseGFXScale;

	private Vector2 ForceStopDurationRange = new Vector2(0.5f, 3f);

	public float movementSpeed;

	[HideInInspector]
	public bool isMoving;

	public bool isStopUntillNextRun;

	private Coroutine PullingCo;

	[HideInInspector]
	public int IDInEnemyDataDict;

	private Tween SmallSizing;

	private float lastTimeUpdatedBar;

	public bool isBoss;

	[HideInInspector]
	public int SkinID;

	private bool isDoingHurtAnimation;

	public Sprite NormalOreSprite;

	public Sprite RichOreSprite;

	private bool isDebuffed_DamageTaken;

	private bool isDebuffed_GoldDropped;

	private bool isDebuffed_CallsSkill;

	private bool isDebuffed_SpawnShiny;

	private GameObject DebuffFX_DamageTaken;

	private GameObject DebuffFX_GoldDropped;

	private GameObject DebuffFX_CallsSkill;

	private GameObject DebuffFX_SpawnShiny;

	private static float ChanceToSpawnOnDeath_Unlucky;

	public static int HowManyTimesHitSoundPlayed;

	private Dictionary<int, float> ChanceToPlayHitSound = new Dictionary<int, float>
	{
		{ 200, 100f },
		{ 350, 85f },
		{ 500, 50f },
		{ 1000, 20f },
		{
			int.MaxValue,
			8f
		}
	};

	private List<int> ChancesThresholdToPlayHitSound = new List<int>();

	private Vector2 PosToMoveToward;

	public event Action<int> OnDeath;

	public void TakeInfo()
	{
		EnemyHashcode = base.gameObject.GetHashCode();
		if (!isInit)
		{
			MyRB = GetComponent<Rigidbody2D>();
			MyCol = GetComponent<Collider2D>();
			MyGFXTrans = base.transform.Find("GFX");
			MyCenterTrans = base.transform.Find("Center");
			MyGroundTrans = base.transform;
			EnemyCanvasGO = base.transform.Find("EnemyCanvas").gameObject;
			if (enemyType != EnemyType.Ore)
			{
				monsterAnimator = MyGFXTrans.GetComponent<SkeletonAnimation>();
				monsterAnimator.AnimationState.Event += OnEventAnimation;
				monsterAnimator.AnimationState.Complete += OnAnimationComplete;
				monsterAnimator.AnimationState.Start += OnAnimationStart;
				monsterRenderer = MyGFXTrans.GetComponent<MeshRenderer>();
			}
			else
			{
				oreRenderer = MyGFXTrans.GetComponent<SpriteRenderer>();
			}
			ChancesThresholdToPlayHitSound = new List<int>(ChanceToPlayHitSound.Keys);
		}
		if (SmallSizing != null)
		{
			SmallSizing.Kill();
		}
		base.transform.localScale = Vector3.one;
		ResetValues(IsResetStats: true);
		isInit = true;
		if (enemyType != EnemyType.Ore)
		{
			monsterRenderer.sortingOrder = Mathf.RoundToInt((0f - MyGFXTrans.position.y) * 10f);
			return;
		}
		oreRenderer.color = Color.white;
		oreRenderer.sortingOrder = Mathf.RoundToInt((0f - MyGFXTrans.position.y) * 10f);
	}

	private void ResetValues(bool IsResetStats)
	{
		isDead = false;
		isMoving = false;
		isDoingHurtAnimation = false;
		isStopUntillNextRun = false;
		isDebuffed_DamageTaken = false;
		isDebuffed_GoldDropped = false;
		isDebuffed_CallsSkill = false;
		if (IsResetStats)
		{
			myStats = new EnemyStatsData();
			if (isBoss && enemyType == EnemyType.Ghost)
			{
				myStats.InitEnemyStats(isGhostBoss: true);
			}
			else
			{
				myStats.InitEnemyStats(isGhostBoss: false);
			}
		}
		if (PullingCo != null)
		{
			StopCoroutine(PullingCo);
		}
		StopAllCoroutines();
		EnemyCanvasGO.SetActive(value: false);
		MyCol.enabled = true;
		ChangeAnimation(Animations.idle);
		base.transform.localScale = Vector3.one;
		Vector2 vector = MyGFXTrans.localScale;
		vector.x = Mathf.Abs(vector.x) * (float)((UnityEngine.Random.Range(0, 2) == 0) ? 1 : (-1));
		baseGFXScale = vector;
		MyGFXTrans.localScale = Vector2.zero;
		MyGFXTrans.DOScale(vector, 0.25f).SetEase(Ease.OutBack);
	}

	public void ChangeSkin(int id)
	{
		SkinID = id;
		if (enemyType != EnemyType.Ore)
		{
			monsterAnimator.skeleton.SetSkin("Skin" + id);
			monsterAnimator.skeleton.SetSlotsToSetupPose();
		}
		else
		{
			oreRenderer.sprite = ((id == 0) ? NormalOreSprite : RichOreSprite);
		}
		if (enemyType == EnemyType.Slime && SkinID != 0 && !RunManager.instance.IsBossRun)
		{
			base.transform.localScale = 1.25f * Vector3.one;
		}
	}

	private void HitAnimation_Sprite()
	{
		if (enemyType == EnemyType.Ore && oreRenderer != null)
		{
			MyGFXTrans.DOKill();
			MyGFXTrans.localScale = baseGFXScale;
			DG.Tweening.Sequence s = DOTween.Sequence();
			Color white = Color.white;
			Color endValue = new Color(1f, 0.6f, 0.6f, white.a);
			Vector3 endValue2 = baseGFXScale * 1.2f;
			s.Append(oreRenderer.DOColor(endValue, 0.1f)).Join(MyGFXTrans.DOScale(endValue2, 0.1f)).Append(oreRenderer.DOColor(white, 0.2f))
				.Join(MyGFXTrans.DOScale(baseGFXScale, 0.2f))
				.SetEase(Ease.OutQuad);
			MyGFXTrans.DOShakePosition(0.3f, 0.3f);
		}
	}

	private void OnEventAnimation(TrackEntry trackEntry, Event e)
	{
		if (e.Data.Name == "Footstep")
		{
			FXManager.instance.PlaySound("EnemyWalkSound_" + enemyType);
		}
	}

	public void ChangeAnimation(Animations AnimationName, bool IsLoop = true)
	{
		if (enemyType == EnemyType.Ore || (AnimationName == Animations.hurt && isDoingHurtAnimation) || (isDead && AnimationName != Animations.death))
		{
			return;
		}
		if (AnimationName == Animations.death || AnimationName == Animations.attack || AnimationName == Animations.hurt)
		{
			IsLoop = false;
		}
		monsterAnimator.timeScale = 1f;
		isDoingHurtAnimation = false;
		if (AnimationName == Animations.hurt)
		{
			monsterAnimator.timeScale = 1.5f;
			isDoingHurtAnimation = true;
		}
		if (AnimationName == Animations.idle)
		{
			monsterAnimator.timeScale = 0.7f;
		}
		if (AnimationName == Animations.death)
		{
			monsterAnimator.timeScale = 1.76f;
			if (playerData.instance.MonstersLevel >= 10)
			{
				monsterAnimator.timeScale = 2.5f;
			}
			else if (playerData.instance.MonstersLevel >= 20)
			{
				monsterAnimator.timeScale = 4f;
			}
			else if (playerData.instance.MonstersLevel >= 30)
			{
				monsterAnimator.timeScale = 7f;
			}
		}
		string animationName = AnimationName.ToString();
		monsterAnimator.AnimationState.SetAnimation(0, animationName, IsLoop);
	}

	public void OnAnimationComplete(TrackEntry trackEntry)
	{
		if (trackEntry.Animation.ToString() == Animations.death.ToString())
		{
			StartCoroutine(DeathAnimationFinished_ToSmallSizing());
		}
		if (trackEntry.Animation.ToString() == Animations.hurt.ToString() && !isMoving)
		{
			ChangeAnimation(Animations.idle);
		}
		isDoingHurtAnimation = false;
	}

	public void OnAnimationStart(TrackEntry trackEntry)
	{
	}

	public void AddDebuff(string debuffName)
	{
		if (isDead)
		{
			return;
		}
		switch (debuffName)
		{
		case "DebuffFX_DamageTaken":
			isDebuffed_DamageTaken = true;
			if (DebuffFX_DamageTaken == null)
			{
				DebuffFX_DamageTaken = ObjectPooler.instance.GiveMeObject("DebuffFX_DamageTaken", base.transform, new Vector2(0f, -15f), isLocalPosition: true);
			}
			DebuffFX_DamageTaken.SetActive(value: true);
			break;
		case "DebuffFX_GoldDropped":
			isDebuffed_GoldDropped = true;
			if (DebuffFX_GoldDropped == null)
			{
				DebuffFX_GoldDropped = ObjectPooler.instance.GiveMeObject("DebuffFX_GoldDropped", base.transform, new Vector2(0f, -15f), isLocalPosition: true);
			}
			DebuffFX_GoldDropped.SetActive(value: true);
			break;
		case "DebuffFX_CallsSkill":
			isDebuffed_CallsSkill = true;
			if (DebuffFX_CallsSkill == null)
			{
				DebuffFX_CallsSkill = ObjectPooler.instance.GiveMeObject("DebuffFX_CallsSkill", base.transform, new Vector2(0f, -15f), isLocalPosition: true);
			}
			DebuffFX_CallsSkill.SetActive(value: true);
			break;
		case "DebuffFX_SpawnShiny":
			isDebuffed_SpawnShiny = true;
			if (DebuffFX_SpawnShiny == null)
			{
				DebuffFX_SpawnShiny = ObjectPooler.instance.GiveMeObject("DebuffFX_SpawnShiny", base.transform, new Vector2(0f, -15f), isLocalPosition: true);
			}
			DebuffFX_SpawnShiny.SetActive(value: true);
			break;
		}
	}

	private void RemoveAllDebuffs()
	{
		isDebuffed_DamageTaken = false;
		isDebuffed_GoldDropped = false;
		isDebuffed_CallsSkill = false;
		isDebuffed_SpawnShiny = false;
		if (DebuffFX_DamageTaken != null)
		{
			DebuffFX_DamageTaken.SetActive(value: false);
		}
		if (DebuffFX_GoldDropped != null)
		{
			DebuffFX_GoldDropped.SetActive(value: false);
		}
		if (DebuffFX_CallsSkill != null)
		{
			DebuffFX_CallsSkill.SetActive(value: false);
		}
		if (DebuffFX_SpawnShiny != null)
		{
			DebuffFX_SpawnShiny.SetActive(value: false);
		}
	}

	public bool isFullHealth()
	{
		return myStats.CurrentHealth >= myStats.Health.Total.RealValue;
	}

	public float GetHealthPercentage()
	{
		return (float)myStats.CurrentHealth / (float)myStats.Health.Total.RealValue;
	}

	private IEnumerator TwiceHit(DamageData DataOfDamage)
	{
		yield return new WaitForSeconds(0.2f);
		TakeDamage(DataOfDamage, isOriginalHit: false);
	}

	public void TakeDamage(DamageData DataOfDamage, bool isOriginalHit = true, bool isMouseAttack = false, bool isForceSound = false)
	{
		if (isDead)
		{
			return;
		}
		DataOfDamage.RandomizeDamage();
		if (!EnemyCanvasGO.activeInHierarchy)
		{
			EnemyCanvasGO.SetActive(value: true);
		}
		if (!isMoving)
		{
			if (enemyType == EnemyType.Ore)
			{
				HitAnimation_Sprite();
			}
			else
			{
				ChangeAnimation(Animations.hurt);
			}
		}
		double num = DataOfDamage.DamageAmount;
		if (isDebuffed_DamageTaken)
		{
			num *= (double)playerData.instance.stats.Debuff_DamageMultiplier.Total.RealValue;
		}
		double num2 = num - myStats.CurrentHealth;
		myStats.CurrentHealth -= num;
		EnemiesManager.instance.TotalDamageTakenByEnemies += num;
		if (!SkillsManager.instance.TotalDamagePerSource.ContainsKey(DataOfDamage.additionalInfo))
		{
			SkillsManager.instance.TotalDamagePerSource.Add(DataOfDamage.additionalInfo, 0.0);
		}
		SkillsManager.instance.TotalDamagePerSource[DataOfDamage.additionalInfo] += num;
		if (isPlayHitSound() || isForceSound)
		{
			FXManager.instance.PlaySound("EnemyHitNormal");
		}
		HowManyTimesHitSoundPlayed++;
		FloatingNumbersManager.instance.GenerateFloatingNumber(num.ToReadable(), MyGFXTrans.position, "NormalDamageFloater");
		if ((double)(Time.timeSinceLevelLoad - lastTimeUpdatedBar) > 0.03)
		{
			lastTimeUpdatedBar = Time.timeSinceLevelLoad;
			MyHealthBar.ManageBar(myStats.CurrentHealth, myStats.Health.Total.RealValue);
		}
		if (myStats.CurrentHealth < 1.0)
		{
			MyHealthBar.ManageBar(myStats.CurrentHealth, myStats.Health.Total.RealValue);
			if (base.gameObject.activeInHierarchy)
			{
				StartedDying(DataOfDamage.additionalInfo);
				if (num2 > Math.Max(myStats.Health.Total.RealValue * 0.05, 1.0) && playerData.instance.stats.OverkillDamageMultiplier.Total.RealValue > 1f)
				{
					num2 *= ((playerData.instance.stats.OverkillDamageMultiplier.Total.RealValue > 90f) ? 0.9 : ((double)(playerData.instance.stats.OverkillDamageMultiplier.Total.RealValue / 100f)));
					EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy(new List<int> { EnemyHashcode });
					if (randomEnemy != null && !randomEnemy.isDead && RunManager.instance.isRunStarted)
					{
						randomEnemy.TakeDamage(new DamageData(num2, IsCrit: false, "OverkillDamage"), isOriginalHit: false);
					}
				}
				return;
			}
		}
		else if (isOriginalHit && !isMouseAttack)
		{
			float num3 = playerData.instance.stats.ChanceForTwiceHits.Total.RealValue;
			if (num3 > 1f && playerData.instance.MonstersLevel == 1)
			{
				num3 = 25f;
			}
			if (FunctionsNeeded.IsHappened(num3))
			{
				StartCoroutine(TwiceHit(DataOfDamage));
			}
		}
		if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceOnAnyHitToApplyDebuff.Total.RealValue))
		{
			DebuffManager.instance.ApplyRandomDebuffOnEnemy(this);
		}
	}

	private bool isPlayHitSound()
	{
		int num = -1;
		for (int i = 0; i < ChancesThresholdToPlayHitSound.Count; i++)
		{
			if (HowManyTimesHitSoundPlayed < ChancesThresholdToPlayHitSound[i])
			{
				num = ChancesThresholdToPlayHitSound[i];
				break;
			}
		}
		if (num == -1)
		{
			return true;
		}
		return FunctionsNeeded.IsHappened(ChanceToPlayHitSound[num]);
	}

	public void StartedDying(string additionalInfo, bool isForce = false)
	{
		if (PullingCo != null)
		{
			StopCoroutine(PullingCo);
		}
		MyCol.enabled = false;
		isDead = true;
		this.OnDeath?.Invoke(EnemyHashcode);
		MyRB.linearVelocity = Vector2.zero;
		isMoving = false;
		EnemyCanvasGO.SetActive(value: false);
		StartCoroutine(MoveStopEnemy(isMove: false, Vector2.zero, 0f));
		if (!isForce)
		{
			playerData.instance.TotalMonstersKilled_CurrentRun++;
			playerData.instance.TotalMonstersKilled_FullGame++;
			RunManager.instance.ChangeTimer_EnemyDeath(playerData.instance.stats.TimerOnMonsterDeath.Total.RealValue);
			if (isDebuffed_SpawnShiny && playerData.instance.UnlockedSystems[UnlockableSystems.Shiny])
			{
				EnemiesManager.instance.CreateAnEnemy(isStart: false, (Vector2)base.transform.position + new Vector2(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50)), EnemiesManager.ForceMonsterSpawnType.Shiny);
			}
			if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceToSpawnMonsterOnDeath.Total.RealValue + ChanceToSpawnOnDeath_Unlucky))
			{
				ChanceToSpawnOnDeath_Unlucky = 0f;
				EnemiesManager.instance.SpawnASingleMonster();
			}
			else
			{
				ChanceToSpawnOnDeath_Unlucky += playerData.instance.stats.ChanceToSpawnMonsterOnDeath.Total.RealValue;
			}
			if (playerData.instance.UnlockedSystems[UnlockableSystems.Skills])
			{
				if (FunctionsNeeded.IsHappened(playerData.instance.stats.SkillsChanceToTriggerOnMonsterDeath.Total.RealValue))
				{
					SkillBarsManager.instance.CallARandomUnlockedSkill();
				}
				if (isDebuffed_CallsSkill && FunctionsNeeded.IsHappened(playerData.instance.stats.Debuff_SkillChance.Total.RealValue))
				{
					SkillBarsManager.instance.CallARandomUnlockedSkill();
				}
			}
			EnemiesManager.instance.EnemyDied(EnemyHashcode);
			ChangeAnimation(Animations.death);
			StartCoroutine(PlayDeathSound());
			PlayerManager.instance.MonsterDiedGiveRewards(base.transform.position, enemyType, SkinID, isDebuffed_GoldDropped, isBoss);
			UIManager.instance.ManageKillText(isStartRun: false);
			if (enemyType == EnemyType.Ore)
			{
				StartCoroutine(DeathAnimationFinished_ToSmallSizing());
			}
			WellManager.instance.FireTrailEffect(GetPosition(isRandomize: false));
			if (!playerData.instance.isFinishedTheGame && RunManager.instance.IsBossRun && DatabaseManager.NumberOfBossesRequiredToFinishBossRun - playerData.instance.TotalMonstersKilled_CurrentRun <= 0)
			{
				RunManager.instance.FinishTheGame();
			}
		}
		else
		{
			DeathAnimationFinished(isForce: true);
		}
	}

	private IEnumerator PlayDeathSound()
	{
		yield return new WaitForSeconds(0.25f);
		if ((enemyType == EnemyType.Slime || enemyType == EnemyType.Ghost) && (playerData.instance.MonstersLevel <= 5 || FunctionsNeeded.IsHappened((playerData.instance.MonstersLevel <= 7) ? 75 : ((playerData.instance.MonstersLevel <= 14) ? 50 : ((playerData.instance.MonstersLevel <= 30) ? 25 : 10)))))
		{
			FXManager.instance.PlaySound("EnemyDeathSound_" + enemyType);
		}
	}

	private IEnumerator DeathAnimationFinished_ToSmallSizing()
	{
		SmallSizing = base.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.OutQuint);
		yield return new WaitForSeconds(0.15f);
		DeathAnimationFinished();
	}

	public void DeathAnimationFinished(bool isForce = false, bool isDestroy = false)
	{
		RemoveAllDebuffs();
		ObjectPooler.instance.ReturnObjectToPool(base.gameObject, enemyType.ToString() + (isBoss ? "_Boss_Prefab" : "_Normal_Prefab"), isDestroy);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Wall" && PullingCo != null)
		{
			StopCoroutine(PullingCo);
		}
	}

	private void OnMouseDown()
	{
	}

	public void PullMeToLocation(Vector2 Pos, float duration)
	{
		PullingCo = StartCoroutine(PullFunction(Pos, duration));
	}

	public Vector2 GetPosition(bool isRandomize = true, bool isGround = false)
	{
		if (isGround)
		{
			return MyGroundTrans.position + (isRandomize ? ((Vector3)(UnityEngine.Random.insideUnitCircle * 20f)) : Vector3.zero);
		}
		return MyCenterTrans.position + (isRandomize ? ((Vector3)(UnityEngine.Random.insideUnitCircle * 20f)) : Vector3.zero);
	}

	private IEnumerator PullFunction(Vector2 Pos, float duration)
	{
		if (!isDead)
		{
			ForceStop();
			if (Vector2.Distance(Pos, new Vector2(base.transform.position.x, base.transform.position.y)) >= 3f)
			{
				Vector2 vector = Pos - new Vector2(base.transform.position.x, base.transform.position.y);
				MyRB.linearVelocity = vector / duration;
				float seconds = UnityEngine.Random.Range(duration * 0.2f, duration * 0.35f);
				yield return new WaitForSeconds(seconds);
			}
		}
	}

	public void ForceStop(float stopDuration = -1f)
	{
		if (isMoving)
		{
			ChangeAnimation(Animations.idle);
		}
		isMoving = false;
		MyRB.linearVelocity = Vector2.zero;
	}

	public IEnumerator MoveStopEnemy(bool isMove, Vector2 PosToMove, float WaitTime)
	{
		if ((isStopUntillNextRun && isMove) || enemyType == EnemyType.TreasureChest || enemyType == EnemyType.Ore)
		{
			yield break;
		}
		if (!isMove)
		{
			isMoving = isMove;
			MyRB.linearVelocity = Vector2.zero;
			ChangeAnimation(Animations.idle);
		}
		else
		{
			yield return new WaitForSeconds(WaitTime);
			if (isDead)
			{
				yield break;
			}
			isMoving = isMove;
			PosToMoveToward = PosToMove;
			Vector2 normalized = (PosToMoveToward - (Vector2)base.transform.position).normalized;
			MyRB.linearVelocity = normalized * movementSpeed;
			Vector2 vector = MyGFXTrans.localScale;
			vector.x = Mathf.Abs(vector.x) * Mathf.Sign(normalized.x) * -1f;
			MyGFXTrans.localScale = vector;
			ChangeAnimation(Animations.walk);
		}
		monsterRenderer.sortingOrder = Mathf.RoundToInt((0f - MyGFXTrans.position.y) * 10f);
	}

	private void Update()
	{
		if (!isDead)
		{
			if (isMoving && Vector2.Distance(PosToMoveToward, base.transform.position) <= 10f)
			{
				StartCoroutine(MoveStopEnemy(isMove: false, Vector2.zero, 0f));
			}
			if (isMoving)
			{
				monsterRenderer.sortingOrder = Mathf.RoundToInt((0f - MyGFXTrans.position.y) * 10f);
			}
		}
	}
}
public enum Animations
{
	idle,
	attack,
	death,
	hurt,
	walk
}
[CreateAssetMenu]
public class ShinyInfo : ScriptableObject
{
	[HideInInspector]
	public string FunctionName;

	public int MaxLevel;

	public Sprite Icon;

	public int SkinID;

	public int OrderOfAppearance;

	public int ExtraStatUnlockCount;

	public ShinyRarity Rarity;

	public Vector2 Shadow_Pos;

	public Vector2 Shadow_Size;

	public string MainStatValueEquation;

	public StatInfo MainStat;

	public string ExtraStatValueEquation;

	public StatInfo ExtraStat;
}
public enum ShinyRarity
{
	Normal,
	Rare,
	Epic
}
public class ShinyManager : MonoBehaviour
{
	public static ShinyManager instance;

	public Transform ShinyUIParent;

	public GameObject SelectShinyTextGo;

	private Dictionary<string, TextMeshProUGUI> NameTexts = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, GameObject> HighlighterGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, Image> IconImages = new Dictionary<string, Image>();

	private Dictionary<string, TextMeshProUGUI> DescriptionTexts = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, TextMeshProUGUI> CountsTexts = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, RectTransform> ShadowRects = new Dictionary<string, RectTransform>();

	private Dictionary<string, Breather> Breathers = new Dictionary<string, Breather>();

	public Color LockedColor;

	public Color UnlockedColor;

	public Color ExtraStatLockedColor;

	public Color RequiresColor;

	private Dictionary<int, ShinyInfo> ShinyOrder = new Dictionary<int, ShinyInfo>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		foreach (ShinyInfo shiny in DatabaseManager.ShinyList)
		{
			ShinyOrder.Add(shiny.OrderOfAppearance, shiny);
		}
		int num = 0;
		foreach (Transform item in ShinyUIParent)
		{
			NameTexts.Add(ShinyOrder[num].FunctionName, item.Find("NamePlace").Find("NameText").GetComponent<TextMeshProUGUI>());
			IconImages.Add(ShinyOrder[num].FunctionName, item.Find("MonsterIcon").GetComponent<Image>());
			DescriptionTexts.Add(ShinyOrder[num].FunctionName, item.Find("DescText").GetComponent<TextMeshProUGUI>());
			CountsTexts.Add(ShinyOrder[num].FunctionName, item.Find("CountText").GetComponent<TextMeshProUGUI>());
			ShadowRects.Add(ShinyOrder[num].FunctionName, item.Find("Shadow").GetComponent<RectTransform>());
			HighlighterGOs.Add(ShinyOrder[num].FunctionName, item.Find("Highlighter").gameObject);
			Breathers.Add(ShinyOrder[num].FunctionName, item.GetComponent<Breather>());
			string shinyName = ShinyOrder[num].FunctionName;
			item.GetComponent<Button>().onClick.AddListener(delegate
			{
				ClickedOnShiny(shinyName);
			});
			item.GetComponent<OnHoverUI>().Init();
			num++;
		}
		SelectShinyTextGo.SetActive(playerData.instance.stats.Well_ShinyCanBeChosenToFarm.Total.RealValue > 50);
		foreach (ShinyInfo shiny2 in DatabaseManager.ShinyList)
		{
			UpdateSingleShiny(shiny2);
		}
	}

	public void UpdateSingleShiny(ShinyInfo shiny)
	{
		IconImages[shiny.FunctionName].sprite = shiny.Icon;
		FunctionsNeeded.ConstrainImageSize(IconImages[shiny.FunctionName].rectTransform, IconImages[shiny.FunctionName], 75f, 250f);
		int num = playerData.instance.ShinyCounts[shiny.FunctionName];
		if (num == 0)
		{
			NameTexts[shiny.FunctionName].text = "? ? ?";
			DescriptionTexts[shiny.FunctionName].text = LocalizerManager.GetTranslatedValue(shiny.Rarity.ToString() + "_Shiny_Text");
			IconImages[shiny.FunctionName].color = LockedColor;
			CountsTexts[shiny.FunctionName].text = "";
			ShadowRects[shiny.FunctionName].gameObject.SetActive(value: false);
		}
		else
		{
			NameTexts[shiny.FunctionName].text = LocalizerManager.GetTranslatedValue(shiny.name + "_Name");
			IconImages[shiny.FunctionName].sprite = shiny.Icon;
			if (shiny.MainStat != null)
			{
				double item = ExpressionEvaluator.Evaluate(shiny.MainStatValueEquation, num);
				string valueDescText_SingleOrMultipleValues = shiny.MainStat.GetValueDescText_SingleOrMultipleValues(new List<double> { item }, isColoredTag: false);
				double num2 = ExpressionEvaluator.Evaluate(shiny.ExtraStatValueEquation, 1.0);
				if (playerData.instance.stats.Well_ShinyCanBeChosenToFarm.Total.RealValue > 50 && playerData.instance.SelectedShiny == shiny.FunctionName)
				{
					num2 *= (double)playerData.instance.stats.Well_ChosenShinyEffectMultiplier.Total.RealValue;
				}
				string text = shiny.ExtraStat.GetValueDescText_SingleOrMultipleValues(new List<double> { num2 }, isColoredTag: false);
				if (num < shiny.ExtraStatUnlockCount)
				{
					text = text.ToColored(ExtraStatLockedColor);
					string str = " (" + LocalizerManager.GetTranslatedThenReplaceValues("ShinyRequires_Text", shiny.ExtraStatUnlockCount.ToString()) + ")";
					str = str.ToColored(RequiresColor);
					text = text + "\n" + str;
				}
				DescriptionTexts[shiny.FunctionName].text = valueDescText_SingleOrMultipleValues + "\n\n" + text;
			}
			CountsTexts[shiny.FunctionName].text = num + ((num >= shiny.MaxLevel) ? (" (" + LocalizerManager.GetTranslatedValue("Max_Text") + ")") : "");
			IconImages[shiny.FunctionName].color = UnlockedColor;
			ShadowRects[shiny.FunctionName].sizeDelta = shiny.Shadow_Size;
			ShadowRects[shiny.FunctionName].localPosition = shiny.Shadow_Pos;
			ShadowRects[shiny.FunctionName].gameObject.SetActive(value: true);
		}
		if (playerData.instance.stats.Well_ShinyCanBeChosenToFarm.Total.RealValue > 50 && playerData.instance.SelectedShiny == shiny.FunctionName)
		{
			HighlighterGOs[shiny.FunctionName].SetActive(value: true);
		}
		else
		{
			HighlighterGOs[shiny.FunctionName].SetActive(value: false);
		}
	}

	public void GainedAShiny(ShinyInfo shiny)
	{
		if (playerData.instance.ShinyCounts[shiny.FunctionName] == 0 || (playerData.instance.ShinyCounts[shiny.FunctionName] >= shiny.ExtraStatUnlockCount - 1 && !playerData.instance.ShinyIsAppliedExtraStat[shiny.FunctionName]))
		{
			MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Shiny, isShow: true);
			Breathers[shiny.FunctionName].StartBreathing();
		}
		playerData.instance.ShinyCounts[shiny.FunctionName]++;
		ApplyShinyStats(shiny);
		UpdateSingleShiny(shiny);
	}

	private void ApplyShinyStats(ShinyInfo shiny)
	{
		int num = playerData.instance.ShinyCounts[shiny.FunctionName];
		if (shiny.MainStat != null)
		{
			double num2 = ((num > 0) ? ExpressionEvaluator.Evaluate(shiny.MainStatValueEquation, num - 1) : 0.0);
			double value = ExpressionEvaluator.Evaluate(shiny.MainStatValueEquation, num) - num2;
			playerData.instance.stats.ChangeAStat(shiny.MainStat.VariableName, shiny.MainStat.StatsProp, value, IsAdd: true);
		}
		if (shiny.ExtraStat != null && num >= shiny.ExtraStatUnlockCount && !playerData.instance.ShinyIsAppliedExtraStat[shiny.FunctionName])
		{
			double num3 = ExpressionEvaluator.Evaluate(shiny.ExtraStatValueEquation, 1.0);
			if (playerData.instance.SelectedShiny == shiny.FunctionName)
			{
				num3 *= (double)playerData.instance.stats.Well_ChosenShinyEffectMultiplier.Total.RealValue;
			}
			playerData.instance.stats.ChangeAStat(shiny.ExtraStat.VariableName, shiny.ExtraStat.StatsProp, num3, IsAdd: true);
			playerData.instance.ShinyIsAppliedExtraStat[shiny.FunctionName] = true;
		}
	}

	public void ClickedOnShiny(string shinyName)
	{
		if (playerData.instance.stats.Well_ShinyCanBeChosenToFarm.Total.RealValue <= 50)
		{
			return;
		}
		if (playerData.instance.SelectedShiny != "")
		{
			string selectedShiny = playerData.instance.SelectedShiny;
			HighlighterGOs[selectedShiny].SetActive(value: false);
			if (playerData.instance.ShinyIsAppliedExtraStat[selectedShiny])
			{
				ShinyInfo shinyInfo = DatabaseManager.ShinyDict[selectedShiny];
				double num = ExpressionEvaluator.Evaluate(shinyInfo.ExtraStatValueEquation, 1.0);
				playerData.instance.stats.ChangeAStat(shinyInfo.ExtraStat.VariableName, shinyInfo.ExtraStat.StatsProp, num * (double)playerData.instance.stats.Well_ChosenShinyEffectMultiplier.Total.RealValue, IsAdd: false);
				playerData.instance.stats.ChangeAStat(shinyInfo.ExtraStat.VariableName, shinyInfo.ExtraStat.StatsProp, num, IsAdd: true);
			}
			playerData.instance.SelectedShiny = shinyName;
			UpdateSingleShiny(DatabaseManager.ShinyDict[selectedShiny]);
		}
		playerData.instance.SelectedShiny = shinyName;
		FXManager.instance.PlayUIClickSound();
		HighlighterGOs[shinyName].SetActive(value: true);
		if (playerData.instance.ShinyIsAppliedExtraStat[shinyName])
		{
			ShinyInfo shinyInfo2 = DatabaseManager.ShinyDict[shinyName];
			double num2 = ExpressionEvaluator.Evaluate(shinyInfo2.ExtraStatValueEquation, 1.0);
			playerData.instance.stats.ChangeAStat(shinyInfo2.ExtraStat.VariableName, shinyInfo2.ExtraStat.StatsProp, num2, IsAdd: false);
			playerData.instance.stats.ChangeAStat(shinyInfo2.ExtraStat.VariableName, shinyInfo2.ExtraStat.StatsProp, num2 * (double)playerData.instance.stats.Well_ChosenShinyEffectMultiplier.Total.RealValue, IsAdd: true);
		}
		UpdateSingleShiny(DatabaseManager.ShinyDict[shinyName]);
	}

	public void PurchasedSelectShiny()
	{
		SelectShinyTextGo.SetActive(playerData.instance.stats.Well_ShinyCanBeChosenToFarm.Total.RealValue > 50);
		foreach (ShinyInfo shiny in DatabaseManager.ShinyList)
		{
			UpdateSingleShiny(shiny);
		}
	}

	public void PurchasedSelectShinyEffectMultiplier(bool isAfter)
	{
		string selectedShiny = playerData.instance.SelectedShiny;
		if (selectedShiny != "" && playerData.instance.ShinyIsAppliedExtraStat[selectedShiny])
		{
			ShinyInfo shinyInfo = DatabaseManager.ShinyDict[selectedShiny];
			double num = ExpressionEvaluator.Evaluate(shinyInfo.ExtraStatValueEquation, 1.0);
			playerData.instance.stats.ChangeAStat(shinyInfo.ExtraStat.VariableName, shinyInfo.ExtraStat.StatsProp, num * (double)playerData.instance.stats.Well_ChosenShinyEffectMultiplier.Total.RealValue, isAfter);
		}
	}
}
public class CrossPlatformFunctions
{
}
public enum GeneralPlatform
{
	Editor,
	PC,
	Console,
	Mobile,
	Server,
	Web,
	Cloud
}
public class DatabaseManager : MonoBehaviour
{
	public List<Color> RaritiesColors = new List<Color>();

	public TreeInfo MainTree;

	public static Dictionary<Rarity, float> RarityChanceToDrop = new Dictionary<Rarity, float>();

	public static ItemStatsInfo AllItemStats;

	public static Dictionary<string, StatInfo> StatsDict = new Dictionary<string, StatInfo>();

	public static List<LevelInfo> LevelList = new List<LevelInfo>();

	public static Dictionary<int, LevelInfo> LevelDict = new Dictionary<int, LevelInfo>();

	public static List<EnemyInfo> EnemyList = new List<EnemyInfo>();

	public static Dictionary<string, EnemyInfo> EnemyDict = new Dictionary<string, EnemyInfo>();

	public static List<AzrarInfo> AzrarList = new List<AzrarInfo>();

	public static Dictionary<string, AzrarInfo> AzrarDict = new Dictionary<string, AzrarInfo>();

	public static List<ProjectileInfo> ProjectileList = new List<ProjectileInfo>();

	public static Dictionary<string, ProjectileInfo> ProjectileDict = new Dictionary<string, ProjectileInfo>();

	public static List<ShinyInfo> ShinyList = new List<ShinyInfo>();

	public static Dictionary<string, ShinyInfo> ShinyDict = new Dictionary<string, ShinyInfo>();

	public static Dictionary<int, ShinyInfo> ShinyDict_SkinID = new Dictionary<int, ShinyInfo>();

	public static List<SkillDetailInfo> SkillDetailList = new List<SkillDetailInfo>();

	public static Dictionary<string, SkillDetailInfo> SkillDetailDict = new Dictionary<string, SkillDetailInfo>();

	public static List<GroundClickableInfo> GroundClickableList = new List<GroundClickableInfo>();

	public static Dictionary<string, GroundClickableInfo> GroundClickableDict = new Dictionary<string, GroundClickableInfo>();

	public static List<CharacterStatInfo> CharacterStatList = new List<CharacterStatInfo>();

	public static Dictionary<string, CharacterStatInfo> CharacterStatDict = new Dictionary<string, CharacterStatInfo>();

	public static List<GemStatInfo> GemStatList = new List<GemStatInfo>();

	public static Dictionary<string, GemStatInfo> GemStatDict = new Dictionary<string, GemStatInfo>();

	public static List<WellInfo> WellList = new List<WellInfo>();

	public static Dictionary<string, WellInfo> WellDict = new Dictionary<string, WellInfo>();

	public static List<TowerInfo> TowerList = new List<TowerInfo>();

	public static Dictionary<string, TowerInfo> TowerDict = new Dictionary<string, TowerInfo>();

	public static MinMax CriticalChanceMinMax = new MinMax(0.0, 100.0);

	public static MinMax CriticalMultiMinMax = new MinMax(50.0, 10000.0);

	public static MinMax DamageTakenMinMax = new MinMax(-500.0, 90.0);

	public static float OneGameUnitToUnityUnit = 10f;

	public static float BaseCriticalMultiplier = 50f;

	public static double BaseDamage = 1.0;

	public static float BaseMouseRadius = 5.5f;

	public static float BaseMouseAttackSpeed = 0.7f;

	public static float BaseTimer = 10f;

	public static int MaxMonstersLevelInGame = 40;

	public static float BaseItemsChance = 8f;

	public static int RareItemLevel = 7;

	public static int EpicItemLevel = 14;

	public static int LegendaryItemLevel = 22;

	public static float BaseNumberOfMonsters = 18f;

	public static int BaseNumberOfMonstersInBossRun = 40;

	public static float BaseTreasureChance = 0.15f;

	public static float BaseTreasureGoldMultiplier = 5f;

	public static float BaseTreasureExpMultiplier = 5f;

	public static float ChainDistance = 1000f;

	public static Dictionary<ShinyRarity, float> ShinyRarityChanceToDrop = new Dictionary<ShinyRarity, float>();

	public static float BaseShinyChance = 0.2f;

	public static float BaseGhostChance = 0.5f;

	public static int BaseGhostDrop_CharacterCurrency = 1;

	public static float BaseOreChance = 0.5f;

	public static int BaseOreChance_Rich = 5;

	public static int BaseOreDrop_GemCurrency = 1;

	public static int RichOreMultiplier = 2;

	public static float BaseSkillsCooldown = 3f;

	public static float BaseArcher_AttackSpeed = 0.9f;

	public static float BaseArcher_DamageMultiplier = 5f;

	public static Dictionary<int, double> EquipmentUnlockCost = new Dictionary<int, double>
	{
		{ 0, 0.0 },
		{ 1, 7000.0 },
		{ 2, 50000000.0 },
		{ 3, 900000000000.0 },
		{ 4, 30000000000000000.0 }
	};

	public static Dictionary<int, int> GemUnlockCost = new Dictionary<int, int>
	{
		{ 0, 0 },
		{ 1, 300 },
		{ 2, 750 }
	};

	public static Dictionary<int, Dictionary<int, int>> GemLevelCost = new Dictionary<int, Dictionary<int, int>>
	{
		{
			0,
			new Dictionary<int, int>
			{
				{ 2, 8 },
				{ 3, 60 },
				{ 4, 150 },
				{ 5, 280 },
				{ 6, 600 },
				{ 7, 700 }
			}
		},
		{
			1,
			new Dictionary<int, int>
			{
				{ 2, 60 },
				{ 3, 150 },
				{ 4, 200 },
				{ 5, 300 },
				{ 6, 600 },
				{ 7, 700 }
			}
		},
		{
			2,
			new Dictionary<int, int>
			{
				{ 2, 150 },
				{ 3, 200 },
				{ 4, 300 },
				{ 5, 450 },
				{ 6, 600 },
				{ 7, 700 }
			}
		}
	};

	public static int GemRerollCost = 10;

	public static float BaseGroundClickableCheckEverySecond = 5f;

	public static float BaseChanceToSpawnBounty = 5f;

	public static float BaseDebuffCheckEverySecond = 5f;

	public static float BaseChanceToSpawnDebuff = 10f;

	public static float BaseDebuffRadius = 12f;

	public static float BaseDebuff_SkillChance = 30f;

	public static float MaxTimerOnMonsterDeath = 10f;

	public static int MaxWellFillCount = 10;

	public static int NumberOfBossesRequiredToFinishBossRun = 100;

	public static float BaseTowerGold_GoldMultiplier = 5f;

	public static int BaseTowerAoE_NumberOfProjectiles = 4;

	public static int NumberOfEffectsForEmpoweredSkills = 5;

	public static float XAmountToManyXGoldGained = 5f;

	public static Dictionary<int, double> EnemyHealthPerLevel = new Dictionary<int, double>
	{
		{ 1, 3.0 },
		{ 2, 6.0 },
		{ 3, 9.0 },
		{ 4, 15.0 },
		{ 5, 36.0 },
		{ 6, 72.0 },
		{ 7, 120.0 },
		{ 8, 195.0 },
		{ 9, 290.0 },
		{ 10, 800.0 },
		{ 11, 1664.0 },
		{ 12, 3416.0 },
		{ 13, 6860.0 },
		{ 14, 15260.0 },
		{ 15, 30760.0 },
		{ 16, 63000.0 },
		{ 17, 120000.0 },
		{ 18, 280000.0 },
		{ 19, 700000.0 },
		{ 20, 1800000.0 },
		{ 21, 4000000.0 },
		{ 22, 10000000.0 },
		{ 23, 24000000.0 },
		{ 24, 55000000.0 },
		{ 25, 130000000.0 },
		{ 26, 300000000.0 },
		{ 27, 670000000.0 },
		{ 28, 1300000000.0 },
		{ 29, 3100000000.0 },
		{ 30, 8300000000.0 },
		{ 31, 25000000000.0 },
		{ 32, 65000000000.0 },
		{ 33, 185000000000.0 },
		{ 34, 560000000000.0 },
		{ 35, 1600000000000.0 },
		{ 36, 5100000000000.0 },
		{ 37, 15500000000000.0 },
		{ 38, 46000000000000.0 },
		{ 39, 145000000000000.0 },
		{ 40, 530000000000000.0 },
		{ 41, 12720000000000000.0 }
	};

	public static Dictionary<int, double> EnemyGoldPerLevel = new Dictionary<int, double>
	{
		{ 1, 1.0 },
		{ 2, 2.0 },
		{ 3, 3.0 },
		{ 4, 5.0 },
		{ 5, 7.0 },
		{ 6, 12.0 },
		{ 7, 18.0 },
		{ 8, 27.0 },
		{ 9, 44.0 },
		{ 10, 74.0 },
		{ 11, 140.0 },
		{ 12, 285.0 },
		{ 13, 550.0 },
		{ 14, 1140.0 },
		{ 15, 2280.0 },
		{ 16, 4560.0 },
		{ 17, 9120.0 },
		{ 18, 23000.0 },
		{ 19, 47000.0 },
		{ 20, 90000.0 },
		{ 21, 160000.0 },
		{ 22, 300000.0 },
		{ 23, 600000.0 },
		{ 24, 1300000.0 },
		{ 25, 2650000.0 },
		{ 26, 5600000.0 },
		{ 27, 12000000.0 },
		{ 28, 25000000.0 },
		{ 29, 52000000.0 },
		{ 30, 144000000.0 },
		{ 31, 385000000.0 },
		{ 32, 1000000000.0 },
		{ 33, 2300000000.0 },
		{ 34, 5400000000.0 },
		{ 35, 14000000000.0 },
		{ 36, 31000000000.0 },
		{ 37, 65000000000.0 },
		{ 38, 144000000000.0 },
		{ 39, 300000000000.0 },
		{ 40, 870000000000.0 }
	};

	public static Dictionary<int, int> NumberOfMonstersToUnlock = new Dictionary<int, int>
	{
		{ 1, 0 },
		{ 2, 11 },
		{ 3, 18 },
		{ 4, 30 },
		{ 5, 45 },
		{ 6, 60 },
		{ 7, 80 },
		{ 8, 105 },
		{ 9, 145 },
		{ 10, 190 },
		{ 11, 215 },
		{ 12, 290 },
		{ 13, 375 },
		{ 14, 455 },
		{ 15, 470 },
		{ 16, 505 },
		{ 17, 530 },
		{ 18, 580 },
		{ 19, 600 },
		{ 20, 620 },
		{ 21, 630 },
		{ 22, 630 },
		{ 23, 650 },
		{ 24, 660 },
		{ 25, 680 },
		{ 26, 710 },
		{ 27, 730 },
		{ 28, 740 },
		{ 29, 770 },
		{ 30, 790 },
		{ 31, 850 },
		{ 32, 870 },
		{ 33, 890 },
		{ 34, 910 },
		{ 35, 930 },
		{ 36, 950 },
		{ 37, 970 },
		{ 38, 990 },
		{ 39, 1000 },
		{ 40, 1000 }
	};

	public static Dictionary<int, double> DamageValuePerLevel = new Dictionary<int, double>
	{
		{ 1, 1.0 },
		{ 2, 1.0 },
		{ 3, 3.0 },
		{ 4, 6.0 },
		{ 5, 12.0 },
		{ 6, 16.0 },
		{ 7, 25.0 },
		{ 8, 45.0 },
		{ 9, 85.0 },
		{ 10, 95.0 },
		{ 11, 110.0 },
		{ 12, 185.0 },
		{ 13, 235.0 },
		{ 14, 355.0 },
		{ 15, 400.0 },
		{ 16, 540.0 },
		{ 17, 810.0 },
		{ 18, 980.0 },
		{ 19, 1170.0 },
		{ 20, 1410.0 },
		{ 21, 1700.0 },
		{ 22, 2025.0 },
		{ 23, 2430.0 },
		{ 24, 2920.0 },
		{ 25, 4000.0 },
		{ 26, 4800.0 },
		{ 27, 5800.0 },
		{ 28, 7400.0 },
		{ 29, 9250.0 },
		{ 30, 14000.0 },
		{ 31, 17500.0 },
		{ 32, 22000.0 },
		{ 33, 27500.0 },
		{ 34, 34500.0 },
		{ 35, 44000.0 },
		{ 36, 54000.0 },
		{ 37, 67500.0 },
		{ 38, 85000.0 },
		{ 39, 110000.0 },
		{ 40, 140000.0 },
		{ 41, 172000.0 },
		{ 42, 215000.0 },
		{ 43, 265000.0 },
		{ 44, 325000.0 },
		{ 45, 460000.0 },
		{ 46, 650000.0 },
		{ 47, 820000.0 },
		{ 48, 1100000.0 },
		{ 49, 1400000.0 },
		{ 50, 1750000.0 },
		{ 51, 2200000.0 },
		{ 52, 2800000.0 },
		{ 53, 3500000.0 },
		{ 54, 4550000.0 },
		{ 55, 5800000.0 },
		{ 56, 7600000.0 },
		{ 57, 9700000.0 },
		{ 58, 12600000.0 },
		{ 59, 15800000.0 },
		{ 60, 19800000.0 },
		{ 61, 24800000.0 },
		{ 62, 31000000.0 },
		{ 63, 40000000.0 },
		{ 64, 55000000.0 },
		{ 65, 70000000.0 },
		{ 66, 82000000.0 },
		{ 67, 100000000.0 },
		{ 68, 128000000.0 },
		{ 69, 155000000.0 },
		{ 70, 200000000.0 },
		{ 71, 275000000.0 },
		{ 72, 343000000.0 },
		{ 73, 430000000.0 },
		{ 74, 540000000.0 },
		{ 75, 670000000.0 },
		{ 76, 860000000.0 },
		{ 77, 1100000000.0 },
		{ 78, 1350000000.0 },
		{ 79, 1750000000.0 },
		{ 80, 2200000000.0 },
		{ 81, 2800000000.0 },
		{ 82, 3800000000.0 },
		{ 83, 4800000000.0 },
		{ 84, 6000000000.0 },
		{ 85, 8000000000.0 },
		{ 86, 10000000000.0 },
		{ 87, 12700000000.0 },
		{ 88, 15000000000.0 },
		{ 89, 19500000000.0 },
		{ 90, 24000000000.0 },
		{ 91, 30000000000.0 },
		{ 92, 38000000000.0 },
		{ 93, 55000000000.0 },
		{ 94, 67000000000.0 },
		{ 95, 88000000000.0 },
		{ 96, 110000000000.0 },
		{ 97, 145000000000.0 },
		{ 98, 180000000000.0 },
		{ 99, 250000000000.0 }
	};

	public static Dictionary<int, double> DamageCostPerLevel = new Dictionary<int, double>
	{
		{ 1, 2.0 },
		{ 2, 10.0 },
		{ 3, 50.0 },
		{ 4, 150.0 },
		{ 5, 700.0 },
		{ 6, 1500.0 },
		{ 7, 4500.0 },
		{ 8, 8500.0 },
		{ 9, 14000.0 },
		{ 10, 27000.0 },
		{ 11, 60000.0 },
		{ 12, 80000.0 },
		{ 13, 140000.0 },
		{ 14, 220000.0 },
		{ 15, 550000.0 },
		{ 16, 1000000.0 },
		{ 17, 1500000.0 },
		{ 18, 2100000.0 },
		{ 19, 3700000.0 },
		{ 20, 5300000.0 },
		{ 21, 7300000.0 },
		{ 22, 9100000.0 },
		{ 23, 12000000.0 },
		{ 24, 20000000.0 },
		{ 25, 34000000.0 },
		{ 26, 47000000.0 },
		{ 27, 62000000.0 },
		{ 28, 80000000.0 },
		{ 29, 100000000.0 },
		{ 30, 135000000.0 },
		{ 31, 180000000.0 },
		{ 32, 220000000.0 },
		{ 33, 275000000.0 },
		{ 34, 345000000.0 },
		{ 35, 450000000.0 },
		{ 36, 600000000.0 },
		{ 37, 750000000.0 },
		{ 38, 900000000.0 },
		{ 39, 1200000000.0 },
		{ 40, 1500000000.0 },
		{ 41, 1900000000.0 },
		{ 42, 2500000000.0 },
		{ 43, 2900000000.0 },
		{ 44, 3600000000.0 },
		{ 45, 5000000000.0 },
		{ 46, 8000000000.0 },
		{ 47, 12000000000.0 },
		{ 48, 17000000000.0 },
		{ 49, 22000000000.0 },
		{ 50, 33000000000.0 },
		{ 51, 50000000000.0 },
		{ 52, 72000000000.0 },
		{ 53, 95000000000.0 },
		{ 54, 180000000000.0 },
		{ 55, 225000000000.0 },
		{ 56, 295000000000.0 },
		{ 57, 400000000000.0 },
		{ 58, 520000000000.0 },
		{ 59, 655000000000.0 },
		{ 60, 830000000000.0 },
		{ 61, 1300000000000.0 },
		{ 62, 1800000000000.0 },
		{ 63, 2600000000000.0 },
		{ 64, 3700000000000.0 },
		{ 65, 5000000000000.0 },
		{ 66, 6800000000000.0 },
		{ 67, 9500000000000.0 },
		{ 68, 16000000000000.0 },
		{ 69, 22000000000000.0 },
		{ 70, 30000000000000.0 },
		{ 71, 38000000000000.0 },
		{ 72, 50000000000000.0 },
		{ 73, 65000000000000.0 },
		{ 74, 80000000000000.0 },
		{ 75, 100000000000000.0 },
		{ 76, 125000000000000.0 },
		{ 77, 155000000000000.0 },
		{ 78, 185000000000000.0 },
		{ 79, 255000000000000.0 },
		{ 80, 320000000000000.0 },
		{ 81, 400000000000000.0 },
		{ 82, 530000000000000.0 },
		{ 83, 700000000000000.0 },
		{ 84, 900000000000000.0 },
		{ 85, 1200000000000000.0 },
		{ 86, 1700000000000000.0 },
		{ 87, 2500000000000000.0 },
		{ 88, 3600000000000000.0 },
		{ 89, 4900000000000000.0 },
		{ 90, 6300000000000000.0 },
		{ 91, 8000000000000000.0 },
		{ 92, 10500000000000000.0 },
		{ 93, 14000000000000000.0 },
		{ 94, 18000000000000000.0 },
		{ 95, 25000000000000000.0 },
		{ 96, 30000000000000000.0 },
		{ 97, 38000000000000000.0 },
		{ 98, 47000000000000000.0 },
		{ 99, 60000000000000000.0 }
	};

	public void AwakeMe()
	{
		RarityChanceToDrop.Add(Rarity.Normal, 81f);
		RarityChanceToDrop.Add(Rarity.Rare, 16.6f);
		RarityChanceToDrop.Add(Rarity.Epic, 2f);
		RarityChanceToDrop.Add(Rarity.Legendary, 0.4f);
		ShinyRarityChanceToDrop.Add(ShinyRarity.Normal, 75f);
		ShinyRarityChanceToDrop.Add(ShinyRarity.Rare, 20f);
		ShinyRarityChanceToDrop.Add(ShinyRarity.Epic, 5f);
		ListingAndDictionaring();
	}

	private void ListingAndDictionaring()
	{
		EnemyList = new List<EnemyInfo>(Resources.LoadAll<EnemyInfo>("EnemyInfos"));
		for (int i = 0; i < EnemyList.Count; i++)
		{
			EnemyDict.Add(EnemyList[i].functionName, EnemyList[i]);
		}
		List<StatInfo> list = new List<StatInfo>(Resources.LoadAll<StatInfo>("StatInfos"));
		for (int j = 0; j < list.Count; j++)
		{
			StatsDict.Add(list[j].VariableName + list[j].StatsProp, list[j]);
		}
		List<LevelInfo> list2 = new List<LevelInfo>(Resources.LoadAll<LevelInfo>("LevelInfos"));
		for (int k = 0; k < list2.Count; k++)
		{
			LevelDict.Add(list2[k].Level, list2[k]);
		}
		AzrarList = new List<AzrarInfo>(Resources.LoadAll<AzrarInfo>("AzrarInfos"));
		for (int l = 0; l < AzrarList.Count; l++)
		{
			AzrarList[l].FunctionName = AzrarList[l].name;
			AzrarDict.Add(AzrarList[l].FunctionName, AzrarList[l]);
		}
		ProjectileList = new List<ProjectileInfo>(Resources.LoadAll<ProjectileInfo>("ProjectileInfos"));
		for (int m = 0; m < ProjectileList.Count; m++)
		{
			ProjectileList[m].functionName = ProjectileList[m].name;
			ProjectileDict.Add(ProjectileList[m].functionName, ProjectileList[m]);
		}
		ShinyList = new List<ShinyInfo>(Resources.LoadAll<ShinyInfo>("ShinyInfos"));
		for (int n = 0; n < ShinyList.Count; n++)
		{
			ShinyList[n].FunctionName = ShinyList[n].name;
			ShinyDict.Add(ShinyList[n].FunctionName, ShinyList[n]);
			ShinyDict_SkinID.Add(ShinyList[n].SkinID, ShinyList[n]);
		}
		SkillDetailList = new List<SkillDetailInfo>(Resources.LoadAll<SkillDetailInfo>("SkillDetailInfos"));
		for (int num = 0; num < SkillDetailList.Count; num++)
		{
			SkillDetailList[num].functionName = SkillDetailList[num].name;
			SkillDetailDict.Add(SkillDetailList[num].functionName, SkillDetailList[num]);
		}
		GroundClickableList = new List<GroundClickableInfo>(Resources.LoadAll<GroundClickableInfo>("GroundClickableInfos"));
		for (int num2 = 0; num2 < GroundClickableList.Count; num2++)
		{
			GroundClickableList[num2].functionName = GroundClickableList[num2].name;
			GroundClickableDict.Add(GroundClickableList[num2].functionName, GroundClickableList[num2]);
		}
		AllItemStats = Resources.Load<ItemStatsInfo>("_SingleObjectInfos/AllItemStatsInfos");
		AllItemStats.AwakeMe();
		CharacterStatList = new List<CharacterStatInfo>(Resources.LoadAll<CharacterStatInfo>("CharacterStatInfos"));
		for (int num3 = 0; num3 < CharacterStatList.Count; num3++)
		{
			CharacterStatList[num3].FunctionName = CharacterStatList[num3].name;
			CharacterStatDict.Add(CharacterStatList[num3].FunctionName, CharacterStatList[num3]);
		}
		GemStatList = new List<GemStatInfo>(Resources.LoadAll<GemStatInfo>("GemStatInfos"));
		for (int num4 = 0; num4 < GemStatList.Count; num4++)
		{
			GemStatList[num4].functionName = GemStatList[num4].name;
			GemStatDict.Add(GemStatList[num4].functionName, GemStatList[num4]);
		}
		WellList = new List<WellInfo>(Resources.LoadAll<WellInfo>("WellInfos"));
		for (int num5 = 0; num5 < WellList.Count; num5++)
		{
			WellList[num5].FunctionName = WellList[num5].name;
			WellDict.Add(WellList[num5].FunctionName, WellList[num5]);
		}
		TowerList = new List<TowerInfo>(Resources.LoadAll<TowerInfo>("TowerInfos"));
		for (int num6 = 0; num6 < TowerList.Count; num6++)
		{
			TowerList[num6].functionName = TowerList[num6].name;
			TowerDict.Add(TowerList[num6].functionName, TowerList[num6]);
		}
	}

	private void TreeClearStageNodes()
	{
		string text = "";
		double num = 0.0;
		List<(TreeNodeInfo, double, string, string, List<StatInfo>)> list = new List<(TreeNodeInfo, double, string, string, List<StatInfo>)>();
		foreach (TreeNodeInfo treeNode in MainTree.TreeNodes)
		{
			bool flag = false;
			for (int i = 0; i < treeNode.NodeCostCurrencies.Count; i++)
			{
				if (treeNode.NodeCostCurrencies[i] == Currencies.ClearCurrency)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			List<StatInfo> nodeStat = treeNode.GetNodeStat();
			string item = "";
			for (int j = 0; j < treeNode.NodeCostCurrencies.Count; j++)
			{
				if (treeNode.NodeCostCurrencies[j] == Currencies.Gold)
				{
					item = ((treeNode.NodeCostEquation.Count > j) ? treeNode.NodeCostEquation[j] : "N/A");
					break;
				}
			}
			string item2 = "";
			double item3 = 0.0;
			for (int k = 0; k < treeNode.NodeCostCurrencies.Count; k++)
			{
				if (treeNode.NodeCostCurrencies[k] != Currencies.ClearCurrency)
				{
					continue;
				}
				item2 = ((treeNode.NodeCostEquation.Count > k) ? treeNode.NodeCostEquation[k] : "N/A");
				if (treeNode.NodeCostEquation.Count <= k || string.IsNullOrEmpty(treeNode.NodeCostEquation[k]))
				{
					break;
				}
				if (double.TryParse(treeNode.NodeCostEquation[k], out var result))
				{
					item3 = result;
					break;
				}
				try
				{
					item3 = new Equation(treeNode.NodeCostEquation[k]).EvaluateEquation_Single(1.0, 0);
				}
				catch
				{
					item3 = 0.0;
				}
				break;
			}
			list.Add((treeNode, item3, item2, item, nodeStat));
		}
		list.Sort(((TreeNodeInfo node, double clearCurrencyValue, string clearCurrencyCost, string goldCost, List<StatInfo> nodeStats) a, (TreeNodeInfo node, double clearCurrencyValue, string clearCurrencyCost, string goldCost, List<StatInfo> nodeStats) b) => a.clearCurrencyValue.CompareTo(b.clearCurrencyValue));
		foreach (var item4 in list)
		{
			for (int l = 0; l < item4.Item5.Count; l++)
			{
				StatInfo statInfo = item4.Item5[l];
				string text2 = statInfo.VariableName + statInfo.StatsProp;
				string text3 = ((item4.Item1.NodeValueEquation.Count > l) ? item4.Item1.NodeValueEquation[l] : "N/A");
				num += item4.Item2;
				string text4 = text2 + " + " + text3;
				if (!string.IsNullOrEmpty(item4.Item4))
				{
					text4 = text4 + " + G: " + item4.Item4;
				}
				text4 = text4 + " + CC: " + item4.Item3;
				text = text + text4 + "\n";
			}
		}
		text += $"\nTotal ClearCurrency Cost: {num}";
		UnityEngine.Debug.Log("Tree Clear Stage Nodes:\n" + text);
	}

	private void CheckValuesOfCertainStats(string statVariableName, StatsProperties statProp)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (ItemStat stat in AllItemStats.Stats)
		{
			if (statVariableName == stat.Stat.VariableName && statProp == stat.Stat.StatsProp)
			{
				dictionary.Add("Item", stat.Value.x + " - " + stat.Value.y);
			}
		}
		foreach (CharacterStatInfo characterStat in CharacterStatList)
		{
			if (statVariableName == characterStat.Stat.VariableName && statProp == characterStat.Stat.StatsProp)
			{
				dictionary.Add("Archer", characterStat.ValueEquation);
			}
		}
		foreach (GemStatInfo gemStat in GemStatList)
		{
			if (statVariableName == gemStat.stat.VariableName && statProp == gemStat.stat.StatsProp)
			{
				for (int i = gemStat.AppearGemLevel; i < gemStat.MaxGemLevel_Exclusive; i++)
				{
					dictionary.Add("Gem" + i, gemStat.LevelsAndValues[i].ToString());
				}
			}
		}
		int num = 0;
		foreach (TreeNodeInfo treeNode in MainTree.TreeNodes)
		{
			foreach (StatInfo item in treeNode.GetNodeStat())
			{
				if (statVariableName == item.VariableName && statProp == item.StatsProp)
				{
					dictionary.Add("TreeNode" + num, treeNode.NodeValueEquation[0].ToString() + " Lvl: " + treeNode.NodeMaxLevel);
					num++;
				}
			}
		}
		foreach (ShinyInfo shiny in ShinyList)
		{
			if (statVariableName == shiny.MainStat.VariableName && statProp == shiny.MainStat.StatsProp)
			{
				dictionary.Add("ShinyMainStat", shiny.MainStatValueEquation);
			}
		}
		foreach (ShinyInfo shiny2 in ShinyList)
		{
			if (statVariableName == shiny2.ExtraStat.VariableName && statProp == shiny2.ExtraStat.StatsProp)
			{
				dictionary.Add("ShinyExtraStat", shiny2.ExtraStatValueEquation);
			}
		}
		foreach (WellInfo well in WellList)
		{
			foreach (StatInfo item2 in well.MainStat)
			{
				int num2 = 0;
				if (statVariableName == item2.VariableName && statProp == item2.StatsProp)
				{
					dictionary.Add("WellMainStat" + num2, well.MainStatValue[num2].ToString());
					num2++;
				}
			}
			foreach (StatInfo item3 in well.ExtraStat)
			{
				int num3 = 0;
				if (statVariableName == item3.VariableName && statProp == item3.StatsProp)
				{
					dictionary.Add("WellExtraStat" + num3, well.ExtraStatValue[num3].ToString());
					num3++;
				}
			}
		}
		dictionary.Debug("Checking values of " + statVariableName + " " + statProp);
	}

	public void AssignNewPlayerData()
	{
		playerData.instance.IsNewPlayer = false;
		playerData.instance.version = SaveLoadManager.CurrentVersion_Expansion + "." + SaveLoadManager.CurrentVersion_Major + "." + SaveLoadManager.CurrentVersion_Minor;
		playerData.instance.randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
		playerData.instance.ChoosenLanguage = LocalizerManager.GetSystemLangauge();
		playerData.instance.stats = new PlayerStatsData();
		playerData.instance.stats.InitData();
		playerData.instance.isFinishedTheGame = false;
		playerData.instance.MonstersLevel = 1;
		playerData.instance.SpecialMonstersPenalty = new StringFloat();
		playerData.instance.SpecialMonstersPenalty.Add("TreasureChest", 1f);
		playerData.instance.SpecialMonstersPenalty.Add("Shiny", 1f);
		playerData.instance.SpecialMonstersPenalty.Add("Ghost", 1f);
		playerData.instance.SpecialMonstersPenalty.Add("Ore", 1f);
		playerData.instance.SpecialMonstersUnluckyProtection = new StringFloat();
		playerData.instance.SpecialMonstersUnluckyProtection.Add("TreasureChest", 0f);
		playerData.instance.SpecialMonstersUnluckyProtection.Add("Shiny", 0f);
		playerData.instance.SpecialMonstersUnluckyProtection.Add("Ghost", 0f);
		playerData.instance.SpecialMonstersUnluckyProtection.Add("Ore", 0f);
		playerData.instance.TotalCurrenciesGained_CurrentRun = new CurrenciesDouble();
		playerData.instance.TotalCurrenciesGained_FullGame = new CurrenciesDouble();
		foreach (Currencies value in Enum.GetValues(typeof(Currencies)))
		{
			playerData.instance.TotalCurrenciesGained_CurrentRun.Add(value, 0.0);
			playerData.instance.TotalCurrenciesGained_FullGame.Add(value, 0.0);
		}
		playerData.instance.TotalItemsDropped_CurrentRun = new RarityInt();
		playerData.instance.TotalItemsDropped_FullGame = new RarityInt();
		foreach (Rarity value2 in Enum.GetValues(typeof(Rarity)))
		{
			playerData.instance.TotalItemsDropped_CurrentRun.Add(value2, 0);
			playerData.instance.TotalItemsDropped_FullGame.Add(value2, 0);
		}
		playerData.instance.AzrarLevels = new StringInt();
		playerData.instance.AzrarIsUnlocked = new StringBool();
		foreach (AzrarInfo azrar in AzrarList)
		{
			playerData.instance.AzrarLevels.Add(azrar.FunctionName, 0);
			playerData.instance.AzrarIsUnlocked.Add(azrar.FunctionName, value: false);
		}
		playerData.instance.UnlockedSystems = new UnlockableSystemsBool();
		playerData.instance.IsJustUnlockedSystem = new UnlockableSystemsBool();
		foreach (UnlockableSystems value3 in Enum.GetValues(typeof(UnlockableSystems)))
		{
			playerData.instance.UnlockedSystems.Add(value3, value: false);
			playerData.instance.IsJustUnlockedSystem.Add(value3, value: false);
		}
		playerData.instance.SelectedShiny = "";
		playerData.instance.ShinyCounts = new StringInt();
		playerData.instance.ShinyIsAppliedExtraStat = new StringBool();
		playerData.instance.TotalShinyFound_FullGame = new StringInt();
		playerData.instance.TotalShinyFound_CurrentRun = new StringInt();
		foreach (ShinyInfo shiny in ShinyList)
		{
			playerData.instance.ShinyCounts.Add(shiny.FunctionName, 0);
			playerData.instance.ShinyIsAppliedExtraStat.Add(shiny.FunctionName, value: false);
			playerData.instance.TotalShinyFound_FullGame.Add(shiny.FunctionName, 0);
			playerData.instance.TotalShinyFound_CurrentRun.Add(shiny.FunctionName, 0);
		}
		playerData.instance.SkillIsUnlocked = new StringBool();
		playerData.instance.SkillUpgradesLevels = new StringListInt();
		foreach (SkillDetailInfo skillDetail in SkillDetailList)
		{
			playerData.instance.SkillIsUnlocked.Add(skillDetail.functionName, value: false);
			playerData.instance.SkillUpgradesLevels.Add(skillDetail.functionName, new List<int>());
			for (int i = 0; i < skillDetail.StatValueCostEquations.Count; i++)
			{
				playerData.instance.SkillUpgradesLevels[skillDetail.functionName].Add(0);
			}
		}
		playerData.instance.Inventory = new List<SavedItemData>();
		for (int j = 0; j < InventoryManager.NumberOfInventorySlots; j++)
		{
			playerData.instance.Inventory.Add(null);
		}
		playerData.instance.Equipment = new List<SavedItemData>();
		playerData.instance.EquipmentIsUnlocked = new IntBool();
		for (int k = 0; k < InventoryManager.NumberOfEquipmentSlots; k++)
		{
			playerData.instance.Equipment.Add(null);
			playerData.instance.EquipmentIsUnlocked.Add(k, value: false);
			if (EquipmentUnlockCost[k] <= 1.0)
			{
				playerData.instance.EquipmentIsUnlocked[k] = true;
			}
		}
		playerData.instance.GroundClickableIsUnlocked = new StringBool();
		foreach (GroundClickableInfo groundClickable in GroundClickableList)
		{
			playerData.instance.GroundClickableIsUnlocked.Add(groundClickable.functionName, value: false);
		}
		playerData.instance.GroundClickableIsUnlocked["Damage_AoE"] = true;
		playerData.instance.GroundClickableIsUnlocked["Damage_Projectile"] = true;
		playerData.instance.GroundClickableIsUnlocked["Gold_Gain"] = true;
		playerData.instance.CharacterStats_Levels = new StringInt();
		foreach (CharacterStatInfo characterStat in CharacterStatList)
		{
			playerData.instance.CharacterStats_Levels.Add(characterStat.FunctionName, 0);
		}
		playerData.instance.Gems = new IntGemSavedData();
		playerData.instance.GemsIsUnlocked = new IntBool();
		for (int l = 0; l < 3; l++)
		{
			playerData.instance.Gems.Add(l, null);
			playerData.instance.GemsIsUnlocked.Add(l, value: false);
		}
		playerData.instance.TreeNodeLevels = new StringInt();
		foreach (TreeNodeInfo treeNode in MainTree.TreeNodes)
		{
			playerData.instance.TreeNodeLevels.Add(treeNode.NodeID, 0);
		}
		playerData.instance.WellFillCount = 0;
		playerData.instance.WellCurrentExp = 0.0;
		playerData.instance.WellPowerLevels = new StringInt();
		playerData.instance.MonsterLevelWhenWellReset = 1;
		foreach (WellInfo well in WellList)
		{
			playerData.instance.WellPowerLevels.Add(well.FunctionName, 0);
		}
	}

	public static double EnemyHealth(int level)
	{
		double num = 1.0;
		if (RunManager.instance.IsBossRun)
		{
			num = 5.0;
		}
		return EnemyHealthPerLevel[level] * num;
	}

	public static double EnemyGold(int level, bool isIgnoreBossRun = false)
	{
		double num = 1.0;
		if (RunManager.instance.IsBossRun && !isIgnoreBossRun)
		{
			num = 5.0;
		}
		return Math.Ceiling(EnemyGoldPerLevel[level] * num);
	}

	public static double DamageValue(int level)
	{
		if (level <= 0)
		{
			return 0.0;
		}
		return DamageValuePerLevel[level];
	}

	public static double DamageCost(int level)
	{
		return DamageCostPerLevel[level];
	}

	public static double ItemSellPrice(Rarity rarity, int Level)
	{
		double num = 1.0;
		switch (rarity)
		{
		case Rarity.Normal:
			num = 1.0;
			break;
		case Rarity.Rare:
			num = 1.25;
			break;
		case Rarity.Epic:
			num = 1.6;
			break;
		case Rarity.Legendary:
			num = 2.0;
			break;
		}
		return Math.Ceiling(5.0 * EnemyGold(Level) * num * (double)NumberOfMonstersToUnlock[Level] / (double)NumberOfMonstersToUnlock[2]);
	}

	public static double ExpToLevelUpEquation(int level, double multiplier = 1.0)
	{
		if (level > 66)
		{
			level = 66;
		}
		return 220.0 * Math.Pow(1.4, level) * multiplier;
	}

	public static double EnemyExpDrop(int Level, double multiplier = 1.0)
	{
		double x = 1.5 + (double)Mathf.Lerp(0f, 0.21f, (float)Level / 35f);
		return 1.75 * Math.Pow(x, Level - 4) * multiplier;
	}

	public static double Well_ExpToLevelUp(int playerLevel)
	{
		if (playerLevel > 12)
		{
			playerLevel = 12;
		}
		return (double)((playerLevel == 1) ? 5000 : 12000) * Math.Pow(1.86, playerLevel);
	}

	public static double Well_EnemyExpDrop(int monsterLevel)
	{
		if (monsterLevel > 40)
		{
			monsterLevel = 40;
		}
		double x = 1.81 + ((monsterLevel == 40) ? 0.4 : 0.0) + 0.05 * ((double)(monsterLevel - 30) / 15.0);
		return 2.3 * Math.Pow(x, monsterLevel - 30);
	}

	public static int ArcherLevelCost(int level)
	{
		return level * 3;
	}

	public static float GetBiasedRandom(int level)
	{
		level = Mathf.Clamp(level, 5, 40);
		float a = Mathf.Lerp(0f, 0.9f, (float)level / 40f);
		float p = 1f / Mathf.Max(a, 0.001f) - 1f;
		return Mathf.Clamp01(Mathf.Pow(UnityEngine.Random.value, p));
	}

	public static double GainRune_Azrar_Cost(int level)
	{
		int num = Mathf.Min(MaxMonstersLevelInGame, level + 14);
		double num2 = 1.0;
		if (num >= 19)
		{
			num2 = 1.7;
		}
		if (num >= 22)
		{
			num2 = 3.0;
		}
		return Math.Ceiling(num2 * (double)NumberOfMonstersToUnlock[num] * EnemyGold(num));
	}

	public static double SpawnShiny_Azrar_Cost(int level)
	{
		int num = Mathf.Min(MaxMonstersLevelInGame, level + 16);
		double num2 = 1.0;
		if (num >= 19)
		{
			num2 = 2.0;
		}
		if (num >= 22)
		{
			num2 = 4.0;
		}
		if (num >= 28)
		{
			num2 = 6.0;
		}
		if (num >= 40)
		{
			num2 = 13.0;
		}
		return Math.Ceiling(num2 * (double)NumberOfMonstersToUnlock[num] * EnemyGold(num));
	}

	public static double SpawnOre_Azrar_Cost(int level)
	{
		int num = Mathf.Min(MaxMonstersLevelInGame, level + 21);
		double num2 = 3.0;
		if (num >= 25)
		{
			num2 = 5.0;
		}
		if (num >= 25)
		{
			num2 = 7.0;
		}
		if (num >= 40)
		{
			num2 = 17.0;
		}
		return Math.Ceiling(num2 * (double)NumberOfMonstersToUnlock[num] * EnemyGold(num));
	}

	public static double SpawnGhost_Azrar_Cost(int level)
	{
		int num = Mathf.Min(MaxMonstersLevelInGame, level + 27);
		double num2 = 7.0;
		if (num >= 31)
		{
			num2 = 10.0;
		}
		if (num >= 40)
		{
			num2 = 22.0;
		}
		return Math.Ceiling(num2 * (double)NumberOfMonstersToUnlock[num] * EnemyGold(num));
	}

	public static double GainWellCurrency_Azrar_Cost(int level)
	{
		int maxMonstersLevelInGame = MaxMonstersLevelInGame;
		double num = 100.0;
		if (level == 1)
		{
			num = 12.0;
		}
		if (level == 2)
		{
			num = 18.0;
		}
		if (level == 3)
		{
			num = 23.0;
		}
		if (level == 4)
		{
			num = 28.0;
		}
		if (level == 4)
		{
			num = 45.0;
		}
		if (level == 5)
		{
			num = 65.0;
		}
		return Math.Ceiling(num * (double)NumberOfMonstersToUnlock[maxMonstersLevelInGame] * EnemyGold(maxMonstersLevelInGame));
	}
}
public class DamageData
{
	public double baseDamageAmount;

	public double DamageAmount;

	public bool IsCritical;

	public string additionalInfo;

	public DamageData(double amount, bool IsCrit, string additionalInfo)
	{
		baseDamageAmount = amount;
		DamageAmount = amount;
		IsCritical = IsCrit;
		this.additionalInfo = additionalInfo;
	}

	public void RandomizeDamage()
	{
	}
}
public enum Rarity
{
	Normal,
	Rare,
	Epic,
	Legendary
}
[Serializable]
public class Equation
{
	public string equation_n;

	public Equation(string equation_n)
	{
		this.equation_n = equation_n;
	}

	public double EvaluateEquation_Single(double parameterValue, int roundTo)
	{
		return Math.Round(Convert.ToDouble(new Expression(equation_n)
		{
			Parameters = { ["n"] = parameterValue }
		}.Evaluate(null)), roundTo);
	}

	public double EvaluateEquation_Cumulative(int From_Include, int To_Include, int roundTo)
	{
		double num = 0.0;
		for (int i = From_Include; i <= To_Include; i++)
		{
			num += EvaluateEquation_Single(i, roundTo);
		}
		return num;
	}
}
public static class FunctionsNeeded
{
	public static List<string> suffixCharacters = new List<string> { "K", "M", "B", "T", "Q" };

	public static float RoundingFloatToNearest(float Number, float Nearest)
	{
		return Mathf.Round(Number * Nearest) / Nearest;
	}

	public static string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	public static bool IsHappened(float Chance)
	{
		if (Chance < 0f)
		{
			return false;
		}
		if (UnityEngine.Random.Range(0f, 100f) <= Chance)
		{
			return true;
		}
		return false;
	}

	public static int IsHappened_Over100Things(float chance)
	{
		if (chance < 100f)
		{
			if (IsHappened(chance))
			{
				return 1;
			}
			return 0;
		}
		int num = Mathf.FloorToInt(chance / 100f);
		float num2 = chance - (float)(num * 100);
		if (num2 > 0f && IsHappened(num2))
		{
			num++;
		}
		return num;
	}

	public static void ConstrainImageSize(RectTransform rect, Image image, float maximumHeight, float maximumWidth)
	{
		image.SetNativeSize();
		Vector2 sizeDelta = rect.sizeDelta;
		float a = sizeDelta.y / maximumHeight;
		float b = sizeDelta.x / maximumWidth;
		float num = Mathf.Max(a, b);
		if (num > 1f)
		{
			rect.sizeDelta = sizeDelta / num;
		}
	}

	public static Vector2 SizeOfSpriteWithScaling(GameObject TheSprite)
	{
		SpriteRenderer component = TheSprite.GetComponent<SpriteRenderer>();
		Vector3 vector = component.sprite.rect.size / component.sprite.pixelsPerUnit;
		vector.x *= TheSprite.transform.lossyScale.x;
		vector.y *= TheSprite.transform.lossyScale.y;
		return vector;
	}

	public static Vector2 SizeOfSpriteNOScaling(GameObject TheSprite)
	{
		SpriteRenderer component = TheSprite.GetComponent<SpriteRenderer>();
		return component.sprite.rect.size / component.sprite.pixelsPerUnit;
	}

	public static Vector2 SizeOfSpriteNOScaling(Sprite TheSprite)
	{
		return TheSprite.rect.size / TheSprite.pixelsPerUnit;
	}

	public static float CalculateAngle(Vector2 TheVector, bool IsRadian = false)
	{
		float num = 0f;
		if (TheVector.y == 0f && TheVector.x == 0f)
		{
			return num;
		}
		if (TheVector.x >= 0f && TheVector.y >= 0f)
		{
			num = Mathf.Atan(TheVector.y / TheVector.x) * 180f / MathF.PI;
		}
		else if (TheVector.x < 0f && TheVector.y >= 0f)
		{
			num = 180f - Mathf.Atan(Mathf.Abs(TheVector.y / TheVector.x)) * 180f / MathF.PI;
		}
		else if (TheVector.x < 0f && TheVector.y < 0f)
		{
			num = 180f + Mathf.Atan(Mathf.Abs(TheVector.y / TheVector.x)) * 180f / MathF.PI;
		}
		else if (TheVector.x >= 0f && TheVector.y < 0f)
		{
			num = 360f - Mathf.Atan(Mathf.Abs(TheVector.y / TheVector.x)) * 180f / MathF.PI;
		}
		if (IsRadian)
		{
			num = num * MathF.PI / 180f;
		}
		return num;
	}

	public static Vector2 GetRandomDirection()
	{
		float f = UnityEngine.Random.Range(0f, MathF.PI * 2f);
		return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
	}

	public static Vector2 GetNearestPointInsideRectangle(Vector2 point, Vector2 corner1, Vector2 corner2, float padding = 0f)
	{
		Vector2 vector = new Vector2(Mathf.Min(corner1.x, corner2.x), Mathf.Min(corner1.y, corner2.y));
		Vector2 vector2 = new Vector2(Mathf.Max(corner1.x, corner2.x), Mathf.Max(corner1.y, corner2.y));
		vector += new Vector2(padding, padding);
		vector2 -= new Vector2(padding, padding);
		if (vector.x > vector2.x || vector.y > vector2.y)
		{
			return (corner1 + corner2) * 0.5f;
		}
		return new Vector2(Mathf.Clamp(point.x, vector.x, vector2.x), Mathf.Clamp(point.y, vector.y, vector2.y));
	}

	public static int ConvertFromMatrixToIndex(int row, int column, int TotalColumns)
	{
		return (row - 1) * TotalColumns + column;
	}

	public static int ConvertFromIndexToMatrix_GetRow(int Index, int TotalColumns)
	{
		return (int)(Math.Floor((double)(Index - 1) / (double)TotalColumns) + 1.0);
	}

	public static int ConvertFromIndexToMatrix_GetColumn(int Index, int TotalColumns)
	{
		if (Index % TotalColumns == 0)
		{
			return TotalColumns;
		}
		return Index % TotalColumns;
	}

	public static TwoInt ConvertFromIndexToMatrix_GetBoth(int Index, int TotalColumns)
	{
		return new TwoInt(ConvertFromIndexToMatrix_GetRow(Index, TotalColumns), ConvertFromIndexToMatrix_GetColumn(Index, TotalColumns));
	}

	public static string GetTimeFromSecondsToMinutesAndSeconds(int Seconds)
	{
		string text = "";
		if (Seconds >= 60)
		{
			int num = 0;
			while (Seconds >= 60)
			{
				num++;
				Seconds -= 60;
			}
			return num.ToString("00") + ":" + Seconds.ToString("00");
		}
		return "00:" + Seconds.ToString("00");
	}

	public static string GetTimeFromSecondsToHoursAndMinutes(int Seconds)
	{
		string text = "";
		int num = Seconds;
		int num2 = 0;
		if (num >= 60)
		{
			while (num >= 60)
			{
				num2++;
				num -= 60;
			}
			text = num2.ToString();
		}
		int num3 = (int)Mathf.Floor(Seconds / 60);
		if ((int)Mathf.Floor(num3 / 60) < 1)
		{
			if (num3 > 1)
			{
				return num3 + " Minutes";
			}
			return num3 + " Minute";
		}
		double num4 = Math.Round((double)(num3 / 60), 1);
		if (num4 >= 2.0)
		{
			return num4 + " Hours";
		}
		return num4 + " Hour";
	}

	public static string DisplayNumberAsNumberSuffix(double NumberToShow)
	{
		if (playerData.instance.isScientific)
		{
			return NumberToShow.ToString("0.00e0");
		}
		if (suffixCharacters.Count < 10)
		{
			suffixCharacters.AddRange(GenerateStrings(2));
		}
		NumberToShow = Math.Round(NumberToShow);
		int num = 0;
		int num2 = 0;
		string text = "";
		if (NumberToShow >= 10000.0)
		{
			while (NumberToShow >= 1000.0)
			{
				NumberToShow /= 10.0;
				num++;
			}
		}
		num2 = (int)Math.Floor(NumberToShow);
		string text2 = num2.ToString().Substring(0, num2.ToString().Length);
		if (num % 3 == 1)
		{
			text2 = text2.Insert(1, ".");
		}
		else if (num % 3 == 2)
		{
			text2 = text2.Insert(2, ".");
		}
		int num3 = (int)Math.Ceiling((double)num / 3.0) - 1;
		if (num3 >= 0)
		{
			text = ((num3 >= suffixCharacters.Count) ? suffixCharacters[num3 % suffixCharacters.Count] : suffixCharacters[num3]);
		}
		return text2 + text;
	}

	public static Vector2 SampleRandomPointInCircle(Vector2 center, float radius)
	{
		float f = UnityEngine.Random.Range(0f, MathF.PI * 2f);
		float num = radius * Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f));
		return center + new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num;
	}

	public static List<Vector2> SampleEvenlySpacedPointsInCircle(int numPoints, Vector2 center, float radius, int candidatesPerPoint = 10)
	{
		List<Vector2> list = new List<Vector2>();
		if (numPoints <= 0)
		{
			return list;
		}
		Vector2 vector = SampleRandomPointInCircle(center, radius);
		while (!EnemiesManager.instance.IsPointInTrapezoid(vector))
		{
			vector = SampleRandomPointInCircle(center, radius);
		}
		list.Add(vector);
		for (int i = 1; i < numPoints; i++)
		{
			Vector2 item = Vector2.zero;
			float num = 0f;
			for (int j = 0; j < candidatesPerPoint; j++)
			{
				Vector2 vector2 = SampleRandomPointInCircle(center, radius);
				while (!EnemiesManager.instance.IsPointInTrapezoid(vector2))
				{
					vector2 = SampleRandomPointInCircle(center, radius);
				}
				float num2 = float.MaxValue;
				foreach (Vector2 item2 in list)
				{
					float num3 = Vector2.Distance(vector2, item2);
					if (num3 < num2)
					{
						num2 = num3;
					}
				}
				if (num2 > num)
				{
					num = num2;
					item = vector2;
				}
			}
			list.Add(item);
		}
		return list;
	}

	public static List<Vector2> BestCandidateSampling(Vector2 SpawnX_from_to, Vector2 SpawnY_from_to, int N, List<Vector2> BossPoints = null)
	{
		int num = 10;
		List<Vector2> list = new List<Vector2>(N);
		if (BossPoints != null)
		{
			list.AddRange(BossPoints);
			N -= BossPoints.Count;
		}
		for (int i = 0; i < N; i++)
		{
			Vector2 item = Vector2.zero;
			float num2 = -1f;
			for (int j = 0; j < num; j++)
			{
				float x = UnityEngine.Random.Range(SpawnX_from_to.x, SpawnX_from_to.y);
				float y = UnityEngine.Random.Range(SpawnY_from_to.x, SpawnY_from_to.y);
				Vector2 vector = new Vector2(x, y);
				if (i == 0)
				{
					item = vector;
					break;
				}
				float num3 = float.MaxValue;
				foreach (Vector2 item2 in list)
				{
					float num4 = DistanceSquared(vector, item2);
					if (num4 < num3)
					{
						num3 = num4;
					}
				}
				if (num3 > num2)
				{
					num2 = num3;
					item = vector;
				}
			}
			list.Add(item);
		}
		return list;
		static float DistanceSquared(Vector2 a, Vector2 b)
		{
			float num5 = a.x - b.x;
			float num6 = a.y - b.y;
			return num5 * num5 + num6 * num6;
		}
	}

	public static List<(Vector2 center, float radius)> LargestEmptyCirclesWithMinSpacing(Vector2 SpawnX_from_to, Vector2 SpawnY_from_to, List<Vector2> points, float step, int topK, float minSpacing)
	{
		float xMin = SpawnX_from_to.x;
		float xMax = SpawnX_from_to.y;
		float yMin = SpawnY_from_to.x;
		float yMax = SpawnY_from_to.y;
		List<(Vector2, float)> list = new List<(Vector2, float)>();
		for (float num = xMin; num <= xMax; num += step)
		{
			for (float num2 = yMin; num2 <= yMax; num2 += step)
			{
				float item = DistanceToPoints(num, num2);
				list.Add((new Vector2(num, num2), item));
			}
		}
		list.Sort(((Vector2 center, float radius) a, (Vector2 center, float radius) b) => b.radius.CompareTo(a.radius));
		List<(Vector2, float)> list2 = new List<(Vector2, float)>();
		foreach (var item4 in list)
		{
			Vector2 item2 = item4.Item1;
			float item3 = item4.Item2;
			bool flag = false;
			foreach (var item5 in list2)
			{
				if (Vector2.Distance(item2, item5.Item1) - (item3 + item5.Item2) <= minSpacing)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list2.Add(item4);
				if (list2.Count == topK)
				{
					break;
				}
			}
		}
		return list2;
		float DistanceToPoints(float x, float y)
		{
			if (points == null || points.Count == 0)
			{
				return float.PositiveInfinity;
			}
			float num3 = float.PositiveInfinity;
			foreach (Vector2 point in points)
			{
				float num4 = Vector2.Distance(new Vector2(x, y), point);
				if (num4 < num3)
				{
					num3 = num4;
				}
			}
			return num3;
		}
	}

	public static List<int> ClosestNeighborPoints(Dictionary<int, Vector2> points, int topX = 3)
	{
		int count = points.Count;
		if (count < 2)
		{
			if (count == 1)
			{
				return new List<int> { points.Keys.FirstOrDefault() };
			}
			return new List<int>();
		}
		List<(int, Vector2, float, int)> list = new List<(int, Vector2, float, int)>(count);
		foreach (int key in points.Keys)
		{
			Vector2 vector = points[key];
			float num = float.PositiveInfinity;
			int item = -1;
			foreach (int key2 in points.Keys)
			{
				if (key != key2)
				{
					float num2 = Vector2.Distance(vector, points[key2]);
					if (num2 < num)
					{
						num = num2;
						item = key2;
					}
				}
			}
			list.Add((key, vector, num, item));
		}
		list.Sort(((int, Vector2, float, int) a, (int, Vector2, float, int) b) => a.Item3.CompareTo(b.Item3));
		List<int> list2 = new List<int>();
		HashSet<int> hashSet = new HashSet<int>();
		foreach (var (item2, _, _, item3) in list)
		{
			if (list2.Count >= topX)
			{
				break;
			}
			if (!hashSet.Contains(item3))
			{
				list2.Add(item2);
				hashSet.Add(item2);
			}
		}
		return list2;
	}

	public static void GeneratePermutations(List<int> elements, int depth, List<List<int>> results)
	{
		if (depth == elements.Count)
		{
			results.Add(new List<int>(elements));
			return;
		}
		for (int i = depth; i < elements.Count; i++)
		{
			Swap(elements, depth, i);
			GeneratePermutations(elements, depth + 1, results);
			Swap(elements, depth, i);
		}
	}

	public static void Swap(List<int> list, int i, int j)
	{
		int value = list[i];
		list[i] = list[j];
		list[j] = value;
	}

	public static List<(Vector2, Vector2)> MinimumTotalDistance(List<Vector2> set1, List<Vector2> set2)
	{
		int count = set1.Count;
		List<int> list = new List<int>();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		List<List<int>> list2 = new List<List<int>>();
		GeneratePermutations(list, 0, list2);
		float num = float.MaxValue;
		List<(Vector2, Vector2)> result = null;
		foreach (List<int> item in list2)
		{
			float num2 = 0f;
			List<(Vector2, Vector2)> list3 = new List<(Vector2, Vector2)>();
			for (int j = 0; j < count; j++)
			{
				num2 += Vector2.Distance(set1[j], set2[item[j]]);
				list3.Add((set1[j], set2[item[j]]));
			}
			if (num2 < num)
			{
				num = num2;
				result = list3;
			}
		}
		return result;
	}

	public static double RealNumberFromNumberSuffix(double Nums, string TheSuffix)
	{
		double result = Nums;
		UnityEngine.Debug.Log("BUG If suffix higher than max (z)");
		if (TheSuffix != "")
		{
			int num = 0;
			for (int i = 0; i < suffixCharacters.Count; i++)
			{
				if (TheSuffix == suffixCharacters[i])
				{
					num = i;
					break;
				}
			}
			result = Nums * Math.Pow(10.0, (num + 1) * 3);
		}
		return result;
	}

	public static int GetARandomFromList_Normal(List<float> TheList)
	{
		float maxInclusive = TheList.Sum();
		float num = UnityEngine.Random.Range(0f, maxInclusive);
		float num2 = 0f;
		for (int i = 0; i < TheList.Count; i++)
		{
			num2 += TheList[i];
			if (num <= num2)
			{
				return i;
			}
		}
		return 0;
	}

	public static int GetARandomFromDict_Weighted(Dictionary<int, float> Dict, float RarityIncrease)
	{
		float num = Dict.FirstOrDefault().Value;
		int num2 = Dict.Keys.Min();
		foreach (KeyValuePair<int, float> item in Dict)
		{
			if (item.Key != num2)
			{
				num += item.Value * (1f + RarityIncrease / 100f);
			}
		}
		float num3 = UnityEngine.Random.Range(0f, num);
		float num4 = Dict[num2];
		foreach (KeyValuePair<int, float> item2 in Dict)
		{
			if (item2.Key != num2)
			{
				num4 += item2.Value * (1f + RarityIncrease / 100f);
			}
			if (num3 <= num4)
			{
				return item2.Key;
			}
		}
		return 0;
	}

	public static int GetARandomFromDict_Lucky(Dictionary<int, float> Dict, float LuckChance)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < HowManyRolls_UsedForLuck(LuckChance); i++)
		{
			list.Add(GetARandomFromDict_Normal(Dict));
		}
		return list.Max();
	}

	public static float GetARandomBetweenRange_Lucky(float min, float max, float LuckChance)
	{
		List<float> list = new List<float>();
		for (int i = 0; i < HowManyRolls_UsedForLuck(LuckChance); i++)
		{
			list.Add(UnityEngine.Random.Range(min, max));
		}
		return list.Max();
	}

	private static int HowManyRolls_UsedForLuck(float LuckChance)
	{
		float num = LuckChance;
		int num2 = 1;
		while (num > 0f)
		{
			if ((float)UnityEngine.Random.Range(0, 100) <= num)
			{
				num2++;
			}
			num -= 100f;
		}
		return num2;
	}

	public static T GetARandomFromDict_Normal<T>(Dictionary<T, float> Dict)
	{
		float maxInclusive = Dict.Values.Sum();
		float num = UnityEngine.Random.Range(0f, maxInclusive);
		float num2 = 0f;
		foreach (KeyValuePair<T, float> item in Dict)
		{
			num2 += item.Value;
			if (num <= num2)
			{
				return item.Key;
			}
		}
		return Dict.FirstOrDefault().Key;
	}

	public static T GetARandomFromDict_RarityIncrease<T>(Dictionary<T, float> Dict, float RarityIncrease)
	{
		T key = Dict.FirstOrDefault().Key;
		float num = Dict[key];
		foreach (KeyValuePair<T, float> item in Dict)
		{
			if (!object.Equals(item.Key, key))
			{
				num += item.Value * (1f + RarityIncrease / 100f);
			}
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = Dict[key];
		foreach (KeyValuePair<T, float> item2 in Dict)
		{
			if (!object.Equals(item2.Key, key))
			{
				num3 += item2.Value * (1f + RarityIncrease / 100f);
			}
			if (num2 <= num3)
			{
				return item2.Key;
			}
		}
		return key;
	}

	public static Rarity GetARandomRarity_Items(float RarityIncrease = 0f, Rarity minimumRarity = Rarity.Normal, Rarity maximumRarity = Rarity.Legendary)
	{
		if (minimumRarity > maximumRarity)
		{
			minimumRarity = Rarity.Normal;
		}
		float num = DatabaseManager.RarityChanceToDrop[minimumRarity];
		foreach (KeyValuePair<Rarity, float> item in DatabaseManager.RarityChanceToDrop)
		{
			if (item.Key <= maximumRarity && item.Key >= minimumRarity && item.Key != minimumRarity)
			{
				num += item.Value * (1f + RarityIncrease / 100f);
			}
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = DatabaseManager.RarityChanceToDrop[minimumRarity];
		foreach (KeyValuePair<Rarity, float> item2 in DatabaseManager.RarityChanceToDrop)
		{
			if (item2.Key >= minimumRarity)
			{
				if (item2.Key != minimumRarity)
				{
					num3 += item2.Value * (1f + RarityIncrease / 100f);
				}
				if (num2 <= num3)
				{
					return item2.Key;
				}
			}
		}
		return Rarity.Normal;
	}

	public static bool HasColorTags(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (!text.Contains("<color=") || !text.Contains("</color>"))
		{
			return false;
		}
		int startIndex = 0;
		while (true)
		{
			int num = text.IndexOf("<color=", startIndex);
			if (num == -1)
			{
				break;
			}
			int num2 = text.IndexOf(">", num);
			if (num2 == -1)
			{
				break;
			}
			int num3 = text.IndexOf("</color>", num2);
			if (num3 == -1)
			{
				break;
			}
			if (double.TryParse(text.Substring(num2 + 1, num3 - num2 - 1), out var _))
			{
				return true;
			}
			startIndex = num3 + "</color>".Length;
		}
		return false;
	}

	public static double? ExtractDoubleFromColorTags(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (!text.Contains("<color=") || !text.Contains("</color>"))
		{
			return null;
		}
		int startIndex = 0;
		while (true)
		{
			int num = text.IndexOf("<color=", startIndex);
			if (num == -1)
			{
				break;
			}
			int num2 = text.IndexOf(">", num);
			if (num2 == -1)
			{
				break;
			}
			int num3 = text.IndexOf("</color>", num2);
			if (num3 == -1)
			{
				break;
			}
			if (double.TryParse(text.Substring(num2 + 1, num3 - num2 - 1), out var result))
			{
				return result;
			}
			startIndex = num3 + "</color>".Length;
		}
		return null;
	}

	public static void AnimateUIElement(bool show, Transform element, Vector2 startPosition, Vector2 endPosition)
	{
		Ease ease = Ease.InOutCubic;
		float duration = 0.2f;
		if (show)
		{
			element.gameObject.SetActive(value: true);
			element.transform.localScale = Vector2.zero;
			element.transform.localPosition = startPosition;
			element.transform.DOLocalMove(endPosition, duration).SetEase(ease);
			element.transform.DOScale(Vector2.one, duration).SetEase(ease);
		}
		else
		{
			element.transform.DOLocalMove(startPosition, duration).SetEase(ease);
			element.transform.DOScale(Vector2.zero, duration).SetEase(ease).OnComplete(delegate
			{
				element.gameObject.SetActive(value: false);
			});
		}
	}

	public static float Map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
	{
		float num = OldMax - OldMin;
		float num2 = NewMax - NewMin;
		return (OldValue - OldMin) * num2 / num + NewMin;
	}

	public static Vector3 MouseWorldPosition(float ZPos)
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = ZPos;
		return Camera.main.ScreenToWorldPoint(mousePosition);
	}

	public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
	{
		if (value.CompareTo(min) < 0)
		{
			return min;
		}
		if (value.CompareTo(max) > 0)
		{
			return max;
		}
		return value;
	}

	public static bool IsOdd(int num)
	{
		return num % 2 != 0;
	}

	public static int EnumHighestValue<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<int>().Max();
	}

	public static bool IsDoubleALargerOrEqualB(double a, double b, double epsilon = 1E-05)
	{
		if (a > b || ApproximatelyEqualEpsilon(a, b, epsilon))
		{
			return true;
		}
		return false;
	}

	public static bool IsDoubleALessOrEqualB(double a, double b, double epsilon = 1E-05)
	{
		if (a < b || ApproximatelyEqualEpsilon(a, b, epsilon))
		{
			return true;
		}
		return false;
	}

	public static bool ApproximatelyEqualEpsilon(double a, double b, double epsilon = 1E-05)
	{
		return Math.Abs(a - b) <= epsilon;
	}

	public static List<string> GenerateStrings(int maxLength)
	{
		string chars = "abcdefghijklmnopqrstuvwxyz";
		List<string> result = new List<string>();
		for (int i = 1; i <= maxLength; i++)
		{
			GenerateStringsRecursive("", chars, i, result);
		}
		return result;
	}

	public static void GenerateStringsRecursive(string current, string chars, int remaining, List<string> result)
	{
		if (remaining == 0)
		{
			result.Add(current);
			return;
		}
		for (int i = 0; i < chars.Length; i++)
		{
			GenerateStringsRecursive(current + chars[i], chars, remaining - 1, result);
		}
	}

	public static string FormatTime(int totalSeconds)
	{
		if (totalSeconds < 60)
		{
			return $"{totalSeconds} " + LocalizerManager.GetTranslatedValue("Seconds_String");
		}
		if (totalSeconds < 3600)
		{
			int num = totalSeconds / 60;
			return $"{num} " + LocalizerManager.GetTranslatedValue("Minutes_String");
		}
		int num2 = totalSeconds / 3600;
		if (num2 < 100)
		{
			int num3 = totalSeconds % 3600 / 60;
			string translatedValue = LocalizerManager.GetTranslatedValue("Hours_String");
			string translatedValue2 = LocalizerManager.GetTranslatedValue("Minutes_String");
			return $"{num2} {translatedValue} {num3} {translatedValue2}";
		}
		return ((float)num2 / 24f).ToString("0.0") + LocalizerManager.GetTranslatedValue("Days_String");
	}

	public static string ExpandColorToAdjacentChars(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		string pattern = "<color=#([A-Fa-f0-9]{6})>.*?</color>";
		MatchCollection matchCollection = Regex.Matches(input, pattern);
		if (matchCollection.Count == 0)
		{
			return input;
		}
		string text = input;
		int num = 0;
		foreach (Match item in matchCollection)
		{
			int num2 = item.Index + item.Length + num;
			string value = item.Groups[1].Value;
			string pattern2 = "^((?:</[^>]+>)*)";
			Match match2 = Regex.Match(text.Substring(num2), pattern2);
			string text2 = (match2.Success ? match2.Groups[1].Value : "");
			num2 += text2.Length;
			string pattern3 = "^(<color=#[A-Fa-f0-9]{6}>([^<]*?)</color>|([^\\s<]+))";
			Match match3 = Regex.Match(text.Substring(num2), pattern3);
			if (!match3.Success)
			{
				continue;
			}
			string text3;
			if (!string.IsNullOrEmpty(match3.Groups[2].Value))
			{
				string value2 = match3.Groups[2].Value;
				text3 = "<color=#" + value + ">" + value2 + "</color>";
			}
			else
			{
				if (string.IsNullOrEmpty(match3.Groups[3].Value))
				{
					continue;
				}
				string value3 = match3.Groups[3].Value;
				text3 = "<color=#" + value + ">" + value3 + "</color>";
			}
			text = text.Substring(0, num2) + text3 + text.Substring(num2 + match3.Length);
			num += text3.Length - match3.Length;
		}
		return text;
	}
}
public static class ThreadSafeRandom
{
	[ThreadStatic]
	private static System.Random Local;

	public static System.Random ThisThreadsRandom => Local ?? (Local = new System.Random(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
}
internal static class MyExtensions
{
	public static T DeepCopy<TKey, TValue, T>(this T original) where T : SerializableDictionary<TKey, TValue>, new()
	{
		if (original == null)
		{
			throw new ArgumentNullException("original");
		}
		T val = new T();
		foreach (KeyValuePair<TKey, TValue> item in original)
		{
			val.Add(item.Key, item.Value);
		}
		return val;
	}

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = ThreadSafeRandom.ThisThreadsRandom.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static void Debug<T>(this IList<T> list, string prefix = "")
	{
		string text = prefix + "[";
		for (int i = 0; i < list.Count; i++)
		{
			text += list[i].ToString();
			text = ((i == list.Count - 1) ? (text + "]") : (text + " , "));
		}
		UnityEngine.Debug.Log(text);
	}

	public static void Debug<TKey, TValue>(this IDictionary<TKey, TValue> dict, string prefix = "")
	{
		string text = prefix + "[";
		int num = 0;
		foreach (KeyValuePair<TKey, TValue> item in dict)
		{
			num++;
			text = text + "(" + item.Key.ToString() + " , " + item.Value.ToString() + ")";
			text = ((num >= dict.Count) ? (text + "]") : (text + " , "));
		}
		UnityEngine.Debug.Log(text);
	}

	public static T GetOneRandom<T>(this IList<T> list)
	{
		return list[ThreadSafeRandom.ThisThreadsRandom.Next(0, list.Count)];
	}

	public static List<T> GetRandomItems<T>(this IList<T> list, int HowMany)
	{
		List<T> list2 = list.ToList();
		if (HowMany > list2.Count)
		{
			return list2;
		}
		List<T> list3 = new List<T>();
		for (int i = 0; i < HowMany; i++)
		{
			int index = ThreadSafeRandom.ThisThreadsRandom.Next(0, list2.Count);
			list3.Add(list2[index]);
			list2.RemoveAt(index);
		}
		return list3;
	}

	public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValue> source)
	{
		foreach (KeyValuePair<TKey, TValue> item in source)
		{
			target[item.Key] = item.Value;
		}
	}

	public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		bool flag = true;
		T val = default(T);
		U other = default(U);
		foreach (T item in source)
		{
			if (flag)
			{
				val = item;
				other = selector(val);
				flag = false;
				continue;
			}
			U val2 = selector(item);
			if (val2.CompareTo(other) > 0)
			{
				other = val2;
				val = item;
			}
		}
		if (flag)
		{
			throw new InvalidOperationException("Sequence is empty.");
		}
		return val;
	}

	public static string ToReadable(this double number, bool isForceReadable = false, int RoundToNearest = 0)
	{
		if (number >= 10000.0 || isForceReadable)
		{
			return FunctionsNeeded.DisplayNumberAsNumberSuffix(number);
		}
		return RoundToNearest switch
		{
			0 => number.ToString("0"), 
			1 => number.ToString("0.0"), 
			2 => number.ToString("0.00"), 
			_ => number.ToString("0.000"), 
		};
	}

	public static string ToColoredRarity(this string str, Rarity rarity)
	{
		return "asd";
	}

	public static string ToColored(this string str, Color color)
	{
		return "<color=#" + FunctionsNeeded.ColorToHex(color) + ">" + str + "</color>";
	}

	public static bool Approx(this double a, double b, double epsilon = 0.001)
	{
		if (double.IsInfinity(a) || double.IsInfinity(b))
		{
			return a == b;
		}
		if (double.IsNaN(a) || double.IsNaN(b))
		{
			return false;
		}
		return Math.Abs(a - b) < epsilon;
	}
}
public struct TwoInt
{
	public int a;

	public int b;

	public TwoInt(int a1, int b1)
	{
		a = a1;
		b = b1;
	}

	public static bool operator ==(TwoInt c1, TwoInt c2)
	{
		return c1.Equals(c2);
	}

	public static bool operator !=(TwoInt c1, TwoInt c2)
	{
		return !c1.Equals(c2);
	}

	public override string ToString()
	{
		return "(" + a + " , " + b + ")";
	}
}
public static class EnumHelper
{
	public static List<TEnum> GetAllEnumValues<TEnum>() where TEnum : Enum
	{
		return new List<TEnum>((TEnum[])Enum.GetValues(typeof(TEnum)));
	}
}
public class FunctionsOfThGameManager
{
}
public class ManagerOfTheGame : MonoBehaviour
{
	public static ManagerOfTheGame instance;

	public bool isDebug = true;

	public bool isLoadOnStart;

	public TextMeshProUGUI DebugForMobile;

	public bool isMobile;

	public Color TreeMainStatColor;

	public TreeInfo MainTree;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			GetComponent<DatabaseManager>().AwakeMe();
			GetComponent<LocalizerManager>().AwakeMe();
			GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>().AwakeMe();
			GetComponent<SaveLoadManager>().AwakeMe();
			isDebug = false;
			SaveLoadManager.instance.Load();
			if (playerData.instance.IsNewPlayer)
			{
				GetComponent<DatabaseManager>().AssignNewPlayerData();
			}
			else
			{
				playerData.instance.stats.Init();
				playerData.instance.stats.InitList();
			}
			SaveLoadManager.instance.ManageVersioning();
			GameObject.Find("SettingsManager").GetComponent<SettingsManager>().AwakeMe();
			GameObject.Find("EnemiesManager").GetComponent<EnemiesManager>().AwakeMe();
			GameObject.Find("RunManager").GetComponent<RunManager>().AwakeMe();
			GameObject.Find("MouseAttacker").GetComponent<MouseAttacker>().AwakeMe();
			GameObject.Find("PlayerManager").GetComponent<PlayerManager>().AwakeMe();
			GameObject.Find("FloatingNumbersManager").GetComponent<FloatingNumbersManager>().AwakeMe();
			GameObject.Find("UIManager").GetComponent<UIManager>().AwakeMe();
			GameObject.Find("TutorialManager").GetComponent<TutorialManager>().AwakeMe();
			GameObject.Find("MainMenusManager").GetComponent<MainMenusManager>().AwakeMe();
			GameObject.Find("AzrarManager").GetComponent<AzrarManager>().AwakeMe();
			GameObject.Find("SkillsManager").GetComponent<SkillsManager>().AwakeMe();
			GameObject.Find("ProjectilesManager").GetComponent<ProjectilesManager>().AwakeMe();
			GameObject.Find("GroundEffectsManager").GetComponent<GroundEffectsManager>().AwakeMe();
			GameObject.Find("FXManager").GetComponent<FXManager>().AwakeMe();
			GameObject.Find("ShinyManager").GetComponent<ShinyManager>().AwakeMe();
			GameObject.Find("SkillsUIManager").GetComponent<SkillsUIManager>().AwakeMe();
			GameObject.Find("SkillBarsManager").GetComponent<SkillBarsManager>().AwakeMe();
			GameObject.Find("InventoryManager").GetComponent<InventoryManager>().AwakeMe();
			GameObject.Find("GroundClickableManager").GetComponent<GroundClickableManager>().AwakeMe();
			GameObject.Find("CharacterUIManager").GetComponent<CharacterUIManager>().AwakeMe();
			GameObject.Find("TowersManager").GetComponent<TowersManager>().AwakeMe();
			GameObject.Find("GemsManager").GetComponent<GemsManager>().AwakeMe();
			GameObject.Find("DebuffManager").GetComponent<DebuffManager>().AwakeMe();
			GameObject.Find("WellManager").GetComponent<WellManager>().AwakeMe();
			GameObject.Find("AchievementsManager").GetComponent<AchievementsManager>().AwakeMe();
			GameObject.Find("FinishManager").GetComponent<FinishManager>().AwakeMe();
			GameObject.Find("AutomatorBot").GetComponent<AutomatorBot>().AwakeMe();
			GameObject.Find("LoggingManager").GetComponent<LoggingManager>().AwakeMe();
			GameObject.Find("StatsViewManager").GetComponent<StatsViewManager>().AwakeMe();
			SaveLoadManager.instance.CallLoadingFunction();
			InvokeRepeating("SaveEveryTenMinutesFunction", 100f, 100f);
		}
	}

	private void Start()
	{
		if (isMobile)
		{
			Application.targetFrameRate = 60;
			return;
		}
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = -1;
	}

	public void ReviewGame()
	{
	}

	private void OnApplicationQuit()
	{
		HandleApplicationQuit();
	}

	private void HandleApplicationQuit()
	{
		SaveLoadManager.instance.Save();
	}

	public void ClickedOnDiscord(int whichButton)
	{
		Application.OpenURL("https://discord.gg/Uy83FuCGFa");
	}

	public void ClickedOnSteam(int whichButton)
	{
		string url = "";
		switch (whichButton)
		{
		case 0:
			url = "https://store.steampowered.com/app/4005560/?utm_source=Game&utm_medium=DemoComplete&utm_campaign=Demo";
			break;
		case 1:
			url = "https://store.steampowered.com/app/4005560/?utm_source=Game&utm_medium=MainMenu&utm_campaign=Demo";
			break;
		case 2:
			url = "https://store.steampowered.com/app/4005560/?utm_source=Game&utm_medium=Settings&utm_campaign=Demo";
			break;
		case 3:
			url = "https://store.steampowered.com/app/4005560/?utm_source=Game&utm_medium=Quit&utm_campaign=Demo";
			break;
		case 4:
			url = "https://store.steampowered.com/developer/SamhariaStudios";
			break;
		}
		Application.OpenURL(url);
	}

	public void ClickedOnOtherGames(int whichButton)
	{
		string url = "";
		switch (whichButton)
		{
		case 0:
			url = "https://store.steampowered.com/app/4412000/?utm_source=Maktala&utm_medium=Maktala&utm_campaign=Maktala";
			break;
		case 1:
			url = "https://store.steampowered.com/app/4330860/?utm_source=Maktala&utm_medium=Maktala&utm_campaign=Maktala";
			break;
		}
		Application.OpenURL(url);
	}

	public void HardRestALLData()
	{
	}

	public void QuitGame()
	{
		SaveLoadManager.instance.Save();
		Application.Quit();
	}

	private void SaveEveryTenMinutesFunction()
	{
		ManualSave();
	}

	public void ManualSave()
	{
		SaveLoadManager.instance.Save();
	}

	public void TestLoad()
	{
		SaveLoadManager.instance.Load();
		SaveLoadManager.instance.CallLoadingFunction();
	}
}
public class ObjectPooler : SerializedMonoBehaviour
{
	public static ObjectPooler instance;

	public Dictionary<string, GameObject> ObjectsToBePooled = new Dictionary<string, GameObject>();

	private Dictionary<string, Queue<GameObject>> ObjectsPools = new Dictionary<string, Queue<GameObject>>();

	private Dictionary<string, List<int>> EffectsAndMaximum = new Dictionary<string, List<int>>();

	private Dictionary<string, int> EffectsAndCurrent = new Dictionary<string, int>();

	public List<SoundSelfer> SoundObjectsWithSS = new List<SoundSelfer>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		for (int i = 0; i < DatabaseManager.ProjectileList.Count; i++)
		{
			ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "main", DatabaseManager.ProjectileList[i].mainPrefab);
			if (DatabaseManager.ProjectileList[i].deathPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "death", DatabaseManager.ProjectileList[i].deathPrefab);
			}
			if (DatabaseManager.ProjectileList[i].FireSoundPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "_FireSound", DatabaseManager.ProjectileList[i].FireSoundPrefab);
			}
			if (DatabaseManager.ProjectileList[i].HitSoundPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "_HitSound", DatabaseManager.ProjectileList[i].HitSoundPrefab);
			}
			if (DatabaseManager.ProjectileList[i].skill_chainEffectPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "chainEffect", DatabaseManager.ProjectileList[i].skill_chainEffectPrefab);
			}
			if (DatabaseManager.ProjectileList[i].skill_mainPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "mainSkill", DatabaseManager.ProjectileList[i].skill_mainPrefab);
			}
			if (DatabaseManager.ProjectileList[i].skill_deathPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "deathSkill", DatabaseManager.ProjectileList[i].skill_deathPrefab);
			}
			if (DatabaseManager.ProjectileList[i].skill_FireSoundPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "FireSoundSkill", DatabaseManager.ProjectileList[i].skill_FireSoundPrefab);
			}
			if (DatabaseManager.ProjectileList[i].skill_HitSoundPrefab != null)
			{
				ObjectsToBePooled.Add(DatabaseManager.ProjectileList[i].functionName + "HitSoundSkill", DatabaseManager.ProjectileList[i].skill_HitSoundPrefab);
			}
		}
		foreach (SoundSelfer item in SoundObjectsWithSS)
		{
			ObjectsToBePooled.Add(item.gameObject.name, item.gameObject);
		}
		foreach (KeyValuePair<string, GameObject> item2 in ObjectsToBePooled)
		{
			ObjectsPools.Add(item2.Key, new Queue<GameObject>());
		}
	}

	public GameObject GiveMeObject(string WhichPool, Transform Parent, Vector3 Position, bool isLocalPosition = false)
	{
		if (ObjectsPools[WhichPool].Count == 0)
		{
			GameObject theObject = UnityEngine.Object.Instantiate(ObjectsToBePooled[WhichPool], Parent);
			ReturnObjectToPool(theObject, WhichPool);
		}
		GameObject gameObject = ObjectsPools[WhichPool].Dequeue();
		gameObject.SetActive(value: true);
		gameObject.transform.SetParent(Parent);
		if (isLocalPosition)
		{
			gameObject.transform.localPosition = Position;
		}
		else
		{
			gameObject.transform.position = Position;
		}
		return gameObject;
	}

	public GameObject GiveMeObject(string WhichPool, Transform Parent, Vector3 GlobalPosition, out bool IsInstatiated)
	{
		IsInstatiated = false;
		if (ObjectsPools[WhichPool].Count == 0)
		{
			GameObject theObject = UnityEngine.Object.Instantiate(ObjectsToBePooled[WhichPool], Parent);
			ReturnObjectToPool(theObject, WhichPool);
			IsInstatiated = true;
		}
		GameObject obj = ObjectsPools[WhichPool].Dequeue();
		obj.transform.position = GlobalPosition;
		obj.SetActive(value: true);
		obj.transform.SetParent(Parent);
		return obj;
	}

	public void ReturnObjectToPool(GameObject TheObject, string WhichPool, bool IsDestroy_UsedForObjectsThatAreRarelyUsed = false)
	{
		if (IsDestroy_UsedForObjectsThatAreRarelyUsed)
		{
			UnityEngine.Object.Destroy(TheObject);
			return;
		}
		TheObject.SetActive(value: false);
		ObjectsPools[WhichPool].Enqueue(TheObject);
	}
}
[Serializable]
public class playerData
{
	public static playerData instance = new playerData();

	public bool IsNewPlayer = true;

	public bool IsSoundsOn = true;

	public bool isScientific;

	public float SoundVolume_Effects = 100f;

	public float SoundVolume_UI = 100f;

	public float ScreenShake = 100f;

	public int ResolutionChoice;

	public bool IsFullScreen = true;

	public bool isDamageFloatingShown = true;

	public float EffectsAmount = 100f;

	public Languages ChoosenLanguage;

	public string version;

	public int randomSeed;

	public PlayerStatsData stats;

	public CurrenciesDouble TotalCurrenciesGained_FullGame;

	public CurrenciesDouble TotalCurrenciesGained_CurrentRun;

	public RarityInt TotalItemsDropped_FullGame;

	public RarityInt TotalItemsDropped_CurrentRun;

	public StringInt TotalShinyFound_FullGame;

	public StringInt TotalShinyFound_CurrentRun;

	public int TotalShinyFound;

	public int TotalMonstersKilled_FullGame;

	public int TotalMonstersKilled_CurrentRun;

	public float SecondsPlayed_FullGame;

	public int TotalMouseAttacks_FullGame;

	public int TotalRunsPlayed_FullGame;

	public bool IsAutomaticallySellNormalAndRareItems;

	public int PlayerLevel;

	public double PlayerExp;

	[SerializeField]
	private double playerGold;

	[SerializeField]
	private int clearCurrency;

	[SerializeField]
	private int characterCurrency;

	[SerializeField]
	private int levelPoints;

	[SerializeField]
	private int gemCurrency;

	[SerializeField]
	private int wellCurrency;

	public int MonstersLevel;

	public StringBool GroundClickableIsUnlocked;

	public StringFloat SpecialMonstersPenalty;

	public StringFloat SpecialMonstersUnluckyProtection;

	public StringInt AzrarLevels;

	public StringBool AzrarIsUnlocked;

	public UnlockableSystemsBool UnlockedSystems;

	public UnlockableSystemsBool IsJustUnlockedSystem;

	public StringInt ShinyCounts;

	public StringBool ShinyIsAppliedExtraStat;

	public string SelectedShiny;

	public bool isJustUnlockedRareShiny;

	public bool isJustUnlockedEpicShiny;

	public bool isJustUnlockedAoETower;

	public bool isJustUnlockedGoldTower;

	public bool isJustUnlockedPierceTower;

	public bool isJustUnlockedCircleTower;

	public StringBool SkillIsUnlocked;

	public StringListInt SkillUpgradesLevels;

	public List<SavedItemData> Inventory;

	public List<SavedItemData> Equipment;

	public IntBool EquipmentIsUnlocked;

	public int TotalStatsInItemsEquipped;

	public float UnluckyProtectionTotalForItems;

	public bool isDroppedRareItem;

	public bool isDroppedEpicItem;

	public bool isDroppedLegendaryItem;

	public int TotalBountiesFound_CurrentRun;

	public int TotalAreaMarksApplied_CurrentRun;

	public int CharacterLevel;

	public StringInt CharacterStats_Levels;

	public IntGemSavedData Gems;

	public IntBool GemsIsUnlocked;

	public int TotalGemsLeveledUp;

	public StringInt TreeNodeLevels;

	public int WellFillCount;

	public double WellCurrentExp;

	public StringInt WellPowerLevels;

	public bool isUnlockedBosses;

	public int MonsterLevelWhenWellReset;

	public bool isFinishedTheGame;

	public double PlayerGold
	{
		get
		{
			return playerGold;
		}
		set
		{
			playerGold = value;
			if (playerGold < 0.0)
			{
				playerGold = 0.0;
			}
		}
	}

	public int ClearCurrency
	{
		get
		{
			return clearCurrency;
		}
		set
		{
			clearCurrency = value;
			if (clearCurrency < 0)
			{
				clearCurrency = 0;
			}
		}
	}

	public int CharacterCurrency
	{
		get
		{
			return characterCurrency;
		}
		set
		{
			characterCurrency = value;
			if (characterCurrency < 0)
			{
				characterCurrency = 0;
			}
		}
	}

	public int LevelPoints
	{
		get
		{
			return levelPoints;
		}
		set
		{
			levelPoints = value;
			if (levelPoints < 0)
			{
				levelPoints = 0;
			}
		}
	}

	public int GemCurrency
	{
		get
		{
			return gemCurrency;
		}
		set
		{
			gemCurrency = value;
			if (gemCurrency < 0)
			{
				gemCurrency = 0;
			}
		}
	}

	public int WellCurrency
	{
		get
		{
			return wellCurrency;
		}
		set
		{
			wellCurrency = value;
			if (wellCurrency < 0)
			{
				wellCurrency = 0;
			}
		}
	}
}
[Serializable]
public class StringObject : SerializableDictionary<string, object>
{
}
[Serializable]
public class IntString : SerializableDictionary<int, string>
{
}
[Serializable]
public class IntListString : SerializableDictionary<int, List<string>>
{
}
[Serializable]
public class IntRarity : SerializableDictionary<int, Rarity>
{
}
[Serializable]
public class RarityInt : SerializableDictionary<Rarity, int>
{
}
[Serializable]
public class StringDouble : SerializableDictionary<string, double>
{
}
[Serializable]
public class StringBool : SerializableDictionary<string, bool>
{
}
[Serializable]
public class StringBoolList : SerializableDictionary<string, List<bool>>
{
}
[Serializable]
public class StringFloat : SerializableDictionary<string, float>
{
}
[Serializable]
public class StringInt : SerializableDictionary<string, int>
{
}
[Serializable]
public class IntBool : SerializableDictionary<int, bool>
{
}
[Serializable]
public class IntInt : SerializableDictionary<int, int>
{
}
[Serializable]
public class IntFloat : SerializableDictionary<int, float>
{
}
[Serializable]
public class IntDouble : SerializableDictionary<int, double>
{
}
[Serializable]
public class IntListInt : SerializableDictionary<int, List<int>>
{
}
[Serializable]
public class IntListBool : SerializableDictionary<int, List<bool>>
{
}
[Serializable]
public class IntListRarity : SerializableDictionary<int, List<Rarity>>
{
}
[Serializable]
public class StringListRarity : SerializableDictionary<string, List<Rarity>>
{
}
[Serializable]
public class IntDictStringDouble : SerializableDictionary<int, StringDouble>
{
}
[Serializable]
public class StringListInt : SerializableDictionary<string, List<int>>
{
}
[Serializable]
public class IntListDouble : SerializableDictionary<int, List<double>>
{
}
[Serializable]
public class IntIntDictStringDouble : SerializableDictionary<int, IntDictStringDouble>
{
}
[Serializable]
public class IntDictStringFloat : SerializableDictionary<int, StringFloat>
{
}
[Serializable]
public class StringStatsFloat : SerializableDictionary<string, StatsFloat>
{
}
[Serializable]
public class CurrenciesDouble : SerializableDictionary<Currencies, double>
{
}
[Serializable]
public class UnlockableSystemsBool : SerializableDictionary<UnlockableSystems, bool>
{
}
[Serializable]
public class IntGemSavedData : SerializableDictionary<int, GemSavedData>
{
}
public class SaveLoadManager : MonoBehaviour
{
	public static SaveLoadManager instance;

	private const string Key = "Q3JpcHRvZ3JhZmlhcyBjb20gUmlxamRhZWwgLyBBRVM";

	private string SaveName = "/mak.su";

	public static int CurrentVersion_Expansion = 1;

	public static int CurrentVersion_Major = 0;

	public static int CurrentVersion_Minor = 1;

	public TreeInfo MainTree;

	public bool isLoadCurrentCustomSave;

	private string CustomSaveName_Level
	{
		get
		{
			return PlayerPrefs.GetString("CustomSaveName_Level", "/mak_demo.su");
		}
		set
		{
			PlayerPrefs.SetString("CustomSaveName_Level", value);
		}
	}

	private string CustomSaveName_Seed
	{
		get
		{
			return PlayerPrefs.GetString("CustomSaveName_Seed", "/mak_demo.su");
		}
		set
		{
			PlayerPrefs.SetString("CustomSaveName_Seed", value);
		}
	}

	private void NamedButton()
	{
		if (!string.IsNullOrEmpty(playerData.instance.version))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(Application.persistentDataPath + "/mak_balance_seed" + playerData.instance.randomSeed + "_Lv" + playerData.instance.MonstersLevel + ".su", FileMode.OpenOrCreate);
			string plainString = JsonConvert.SerializeObject(playerData.instance, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			});
			plainString = Encrypt(plainString);
			binaryFormatter.Serialize(fileStream, plainString);
			fileStream.Close();
		}
	}

	public void SaveGame_ClearedLevel()
	{
		if (!string.IsNullOrEmpty(playerData.instance.version))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(Application.persistentDataPath + "/mak_balance_seed" + playerData.instance.randomSeed + "_Lv" + playerData.instance.MonstersLevel + ".su", FileMode.OpenOrCreate);
			string plainString = JsonConvert.SerializeObject(playerData.instance, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			});
			plainString = Encrypt(plainString);
			binaryFormatter.Serialize(fileStream, plainString);
			fileStream.Close();
		}
	}

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
			isLoadCurrentCustomSave = false;
		}
	}

	public void Save()
	{
		if (!string.IsNullOrEmpty(playerData.instance.version))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(Application.persistentDataPath + SaveName, FileMode.OpenOrCreate);
			string plainString = JsonConvert.SerializeObject(playerData.instance, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			});
			plainString = Encrypt(plainString);
			binaryFormatter.Serialize(fileStream, plainString);
			fileStream.Close();
		}
	}

	public void Load()
	{
		string saveName = SaveName;
		_ = isLoadCurrentCustomSave;
		if (File.Exists(Application.persistentDataPath + saveName))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(Application.persistentDataPath + saveName, FileMode.Open);
			string value = Decrypt((string)binaryFormatter.Deserialize(fileStream));
			fileStream.Close();
			playerData.instance = JsonConvert.DeserializeObject<playerData>(value, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			});
		}
	}

	private static TripleDESCryptoServiceProvider GetCryproProvider()
	{
		byte[] key = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes("Q3JpcHRvZ3JhZmlhcyBjb20gUmlxamRhZWwgLyBBRVM"));
		return new TripleDESCryptoServiceProvider
		{
			Key = key,
			Mode = CipherMode.ECB,
			Padding = PaddingMode.PKCS7
		};
	}

	public static string Encrypt(string plainString)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(plainString);
		return Convert.ToBase64String(GetCryproProvider().CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length));
	}

	public static string Decrypt(string encryptedString)
	{
		byte[] array = Convert.FromBase64String(encryptedString);
		byte[] bytes = GetCryproProvider().CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
		return Encoding.UTF8.GetString(bytes);
	}

	public void ManageVersioning()
	{
		string[] array = playerData.instance.version.Split('.');
		int num = int.Parse(array[0]);
		int num2 = int.Parse(array[1]);
		int num3 = int.Parse(array[2]);
		if (num == 1)
		{
			if (num2 == 0)
			{
				_ = 1;
			}
			_ = 1;
		}
		_ = 2;
		playerData.instance.version = CurrentVersion_Expansion + "." + CurrentVersion_Major + "." + CurrentVersion_Minor;
	}

	public void CallLoadingFunction()
	{
	}
}
[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	private static class PrimeHelper
	{
		public static readonly int[] Primes = new int[72]
		{
			3, 7, 11, 17, 23, 29, 37, 47, 59, 71,
			89, 107, 131, 163, 197, 239, 293, 353, 431, 521,
			631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
			4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
			25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363,
			156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
			968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559,
			5999471, 7199369
		};

		public static bool IsPrime(int candidate)
		{
			if (((uint)candidate & (true ? 1u : 0u)) != 0)
			{
				int num = (int)Math.Sqrt(candidate);
				for (int i = 3; i <= num; i += 2)
				{
					if (candidate % i == 0)
					{
						return false;
					}
				}
				return true;
			}
			return candidate == 2;
		}

		public static int GetPrime(int min)
		{
			if (min < 0)
			{
				throw new ArgumentException("min < 0");
			}
			for (int i = 0; i < Primes.Length; i++)
			{
				int num = Primes[i];
				if (num >= min)
				{
					return num;
				}
			}
			for (int j = min | 1; j < int.MaxValue; j += 2)
			{
				if (IsPrime(j) && (j - 1) % 101 != 0)
				{
					return j;
				}
			}
			return min;
		}

		public static int ExpandPrime(int oldSize)
		{
			int num = 2 * oldSize;
			if (num > 2146435069 && 2146435069 > oldSize)
			{
				return 2146435069;
			}
			return GetPrime(num);
		}
	}

	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
	{
		private readonly SerializableDictionary<TKey, TValue> _Dictionary;

		private int _Version;

		private int _Index;

		private KeyValuePair<TKey, TValue> _Current;

		public KeyValuePair<TKey, TValue> Current => _Current;

		object IEnumerator.Current => Current;

		internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
		{
			_Dictionary = dictionary;
			_Version = dictionary._Version;
			_Current = default(KeyValuePair<TKey, TValue>);
			_Index = 0;
		}

		public bool MoveNext()
		{
			if (_Version != _Dictionary._Version)
			{
				throw new InvalidOperationException($"Enumerator version {_Version} != Dictionary version {_Dictionary._Version}");
			}
			while (_Index < _Dictionary._Count)
			{
				if (_Dictionary._HashCodes[_Index] >= 0)
				{
					_Current = new KeyValuePair<TKey, TValue>(_Dictionary._Keys[_Index], _Dictionary._Values[_Index]);
					_Index++;
					return true;
				}
				_Index++;
			}
			_Index = _Dictionary._Count + 1;
			_Current = default(KeyValuePair<TKey, TValue>);
			return false;
		}

		void IEnumerator.Reset()
		{
			if (_Version != _Dictionary._Version)
			{
				throw new InvalidOperationException($"Enumerator version {_Version} != Dictionary version {_Dictionary._Version}");
			}
			_Index = 0;
			_Current = default(KeyValuePair<TKey, TValue>);
		}

		public void Dispose()
		{
		}
	}

	[SerializeField]
	[HideInInspector]
	private int[] _Buckets;

	[SerializeField]
	[HideInInspector]
	private int[] _HashCodes;

	[SerializeField]
	[HideInInspector]
	private int[] _Next;

	[SerializeField]
	[HideInInspector]
	private int _Count;

	[SerializeField]
	[HideInInspector]
	private int _Version;

	[SerializeField]
	[HideInInspector]
	private int _FreeList;

	[SerializeField]
	[HideInInspector]
	private int _FreeCount;

	[SerializeField]
	[HideInInspector]
	public TKey[] _Keys;

	[SerializeField]
	[HideInInspector]
	private TValue[] _Values;

	private readonly IEqualityComparer<TKey> _Comparer;

	public Dictionary<TKey, TValue> AsDictionary => new Dictionary<TKey, TValue>(this);

	public int Count => _Count - _FreeCount;

	public TValue this[TKey key, TValue defaultValue]
	{
		get
		{
			int num = FindIndex(key);
			if (num >= 0)
			{
				return _Values[num];
			}
			return defaultValue;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			int num = FindIndex(key);
			if (num >= 0)
			{
				return _Values[num];
			}
			throw new KeyNotFoundException(key.ToString());
		}
		set
		{
			Insert(key, value, add: false);
		}
	}

	public ICollection<TKey> Keys => (from i in Enumerable.Range(0, _Count)
		where _HashCodes[i] >= 0
		select _Keys[i]).ToArray();

	public ICollection<TValue> Values => (from i in Enumerable.Range(0, _Count)
		where _HashCodes[i] >= 0
		select _Values[i]).ToArray();

	public bool IsReadOnly => false;

	public SerializableDictionary()
		: this(0, (IEqualityComparer<TKey>)null)
	{
	}

	public SerializableDictionary(int capacity)
		: this(capacity, (IEqualityComparer<TKey>)null)
	{
	}

	public SerializableDictionary(IEqualityComparer<TKey> comparer)
		: this(0, comparer)
	{
	}

	public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException("capacity");
		}
		Initialize(capacity);
		_Comparer = comparer ?? EqualityComparer<TKey>.Default;
	}

	public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
		: this(dictionary, (IEqualityComparer<TKey>)null)
	{
	}

	public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		: this(dictionary?.Count ?? 0, comparer)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		foreach (KeyValuePair<TKey, TValue> item in dictionary)
		{
			Add(item.Key, item.Value);
		}
	}

	public bool ContainsValue(TValue value)
	{
		if (value == null)
		{
			for (int i = 0; i < _Count; i++)
			{
				if (_HashCodes[i] >= 0 && _Values[i] == null)
				{
					return true;
				}
			}
		}
		else
		{
			EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			for (int j = 0; j < _Count; j++)
			{
				if (_HashCodes[j] >= 0 && @default.Equals(_Values[j], value))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool ContainsKey(TKey key)
	{
		return FindIndex(key) >= 0;
	}

	public void Clear()
	{
		if (_Count > 0)
		{
			for (int i = 0; i < _Buckets.Length; i++)
			{
				_Buckets[i] = -1;
			}
			Array.Clear(_Keys, 0, _Count);
			Array.Clear(_Values, 0, _Count);
			Array.Clear(_HashCodes, 0, _Count);
			Array.Clear(_Next, 0, _Count);
			_FreeList = -1;
			_Count = 0;
			_FreeCount = 0;
			_Version++;
		}
	}

	public void Add(TKey key, TValue value)
	{
		Insert(key, value, add: true);
	}

	private void Resize(int newSize, bool forceNewHashCodes)
	{
		int[] array = new int[newSize];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = -1;
		}
		TKey[] array2 = new TKey[newSize];
		TValue[] array3 = new TValue[newSize];
		int[] array4 = new int[newSize];
		int[] array5 = new int[newSize];
		Array.Copy(_Values, 0, array3, 0, _Count);
		Array.Copy(_Keys, 0, array2, 0, _Count);
		Array.Copy(_HashCodes, 0, array4, 0, _Count);
		Array.Copy(_Next, 0, array5, 0, _Count);
		if (forceNewHashCodes)
		{
			for (int j = 0; j < _Count; j++)
			{
				if (array4[j] != -1)
				{
					array4[j] = _Comparer.GetHashCode(array2[j]) & 0x7FFFFFFF;
				}
			}
		}
		for (int k = 0; k < _Count; k++)
		{
			int num = array4[k] % newSize;
			array5[k] = array[num];
			array[num] = k;
		}
		_Buckets = array;
		_Keys = array2;
		_Values = array3;
		_HashCodes = array4;
		_Next = array5;
	}

	private void Resize()
	{
		Resize(PrimeHelper.ExpandPrime(_Count), forceNewHashCodes: false);
	}

	public bool Remove(TKey key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		int num = _Comparer.GetHashCode(key) & 0x7FFFFFFF;
		int num2 = num % _Buckets.Length;
		int num3 = -1;
		for (int num4 = _Buckets[num2]; num4 >= 0; num4 = _Next[num4])
		{
			if (_HashCodes[num4] == num && _Comparer.Equals(_Keys[num4], key))
			{
				if (num3 < 0)
				{
					_Buckets[num2] = _Next[num4];
				}
				else
				{
					_Next[num3] = _Next[num4];
				}
				_HashCodes[num4] = -1;
				_Next[num4] = _FreeList;
				_Keys[num4] = default(TKey);
				_Values[num4] = default(TValue);
				_FreeList = num4;
				_FreeCount++;
				_Version++;
				return true;
			}
			num3 = num4;
		}
		return false;
	}

	private void Insert(TKey key, TValue value, bool add)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (_Buckets == null)
		{
			Initialize(0);
		}
		int num = _Comparer.GetHashCode(key) & 0x7FFFFFFF;
		int num2 = num % _Buckets.Length;
		int num3 = 0;
		for (int num4 = _Buckets[num2]; num4 >= 0; num4 = _Next[num4])
		{
			if (_HashCodes[num4] == num && _Comparer.Equals(_Keys[num4], key))
			{
				if (add)
				{
					TKey val = key;
					throw new ArgumentException("Key already exists: " + val);
				}
				_Values[num4] = value;
				_Version++;
				return;
			}
			num3++;
		}
		int num5;
		if (_FreeCount > 0)
		{
			num5 = _FreeList;
			_FreeList = _Next[num5];
			_FreeCount--;
		}
		else
		{
			if (_Count == _Keys.Length)
			{
				Resize();
				num2 = num % _Buckets.Length;
			}
			num5 = _Count;
			_Count++;
		}
		_HashCodes[num5] = num;
		_Next[num5] = _Buckets[num2];
		_Keys[num5] = key;
		_Values[num5] = value;
		_Buckets[num2] = num5;
		_Version++;
	}

	private void Initialize(int capacity)
	{
		int prime = PrimeHelper.GetPrime(capacity);
		_Buckets = new int[prime];
		for (int i = 0; i < _Buckets.Length; i++)
		{
			_Buckets[i] = -1;
		}
		_Keys = new TKey[prime];
		_Values = new TValue[prime];
		_HashCodes = new int[prime];
		_Next = new int[prime];
		_FreeList = -1;
	}

	private int FindIndex(TKey key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (_Buckets != null)
		{
			int num = _Comparer.GetHashCode(key) & 0x7FFFFFFF;
			for (int num2 = _Buckets[num % _Buckets.Length]; num2 >= 0; num2 = _Next[num2])
			{
				if (_HashCodes[num2] == num && _Comparer.Equals(_Keys[num2], key))
				{
					return num2;
				}
			}
		}
		return -1;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		int num = FindIndex(key);
		if (num >= 0)
		{
			value = _Values[num];
			return true;
		}
		value = default(TValue);
		return false;
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		int num = FindIndex(item.Key);
		if (num >= 0)
		{
			return EqualityComparer<TValue>.Default.Equals(_Values[num], item.Value);
		}
		return false;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < 0 || index > array.Length)
		{
			throw new ArgumentOutOfRangeException($"index = {index} array.Length = {array.Length}");
		}
		if (array.Length - index < Count)
		{
			throw new ArgumentException($"The number of elements in the dictionary ({Count}) is greater than the available space from index to the end of the destination array {array.Length}.");
		}
		for (int i = 0; i < _Count; i++)
		{
			if (_HashCodes[i] >= 0)
			{
				array[index++] = new KeyValuePair<TKey, TValue>(_Keys[i], _Values[i]);
			}
		}
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return Remove(item.Key);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return GetEnumerator();
	}
}
public class StateOfTheGameManager
{
}
public class AutomatorBot : MonoBehaviour
{
	public static AutomatorBot instance;

	public float DecisionDelay;

	private Vector2 MousePosition;

	private bool isDeciding;

	private bool isRunStarted;

	public bool isActivateBot;

	private int NumberOfEnemiesInRadius_Start;

	private int NumberOfEnemiesInRadius_Current;

	private float radiusToCheck;

	private bool isThereisBountyAvailable;

	private Vector2 BountyPosition;

	private bool isThereisTowerAvailable;

	private Vector2 TowerPosition;

	public float GameTimeMultiplier = 1f;

	private float MoveAroundCheckEvery = 0.1f;

	private float MoveAroundCheckTimer;

	[DllImport("user32.dll")]
	private static extern bool SetCursorPos(int X, int Y);

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		MousePosition = Vector2.zero;
		isActivateBot = false;
	}

	public void StartRun()
	{
		if (isActivateBot)
		{
			Time.timeScale = GameTimeMultiplier;
			isRunStarted = true;
			radiusToCheck = playerData.instance.stats.MouseRadius.Total.RealValue * DatabaseManager.OneGameUnitToUnityUnit;
			isThereisBountyAvailable = false;
			isThereisTowerAvailable = false;
			isDeciding = false;
			StartCoroutine(TakeDecision());
		}
	}

	public void EndRun()
	{
		if (isActivateBot)
		{
			isRunStarted = false;
			Time.timeScale = 1f;
			Vector3 position = new Vector3(0f, 0f, 0f);
			Vector3 vector = Camera.main.WorldToScreenPoint(position);
			Mouse.current.WarpCursorPosition(vector);
		}
	}

	public void BountyFound(bool isTaken, Vector2 Position)
	{
		if (isActivateBot)
		{
			if (isTaken)
			{
				isThereisBountyAvailable = false;
				return;
			}
			isThereisBountyAvailable = true;
			BountyPosition = Position;
		}
	}

	public void TowerFound(bool isTaken, Vector2 Position)
	{
		if (isActivateBot)
		{
			if (isTaken)
			{
				isThereisTowerAvailable = false;
				return;
			}
			isThereisTowerAvailable = true;
			TowerPosition = Position;
		}
	}

	public void MouseDealtAttack()
	{
		if (isActivateBot && !isDeciding)
		{
			StartCoroutine(TakeDecision());
		}
	}

	private IEnumerator TakeDecision()
	{
		isDeciding = true;
		yield return new WaitForSeconds(DecisionDelay);
		isDeciding = false;
		SearchForANewPosition();
	}

	private void SearchForANewPosition()
	{
		if (!isActivateBot)
		{
			return;
		}
		List<EnemySelfer> enemiesInCircle = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(MousePosition, radiusToCheck);
		NumberOfEnemiesInRadius_Current = enemiesInCircle.Count;
		var (num, mousePosition) = GetGoodMousePosition();
		if (NumberOfEnemiesInRadius_Current == 0)
		{
			MousePosition = mousePosition;
		}
		else if (num >= NumberOfEnemiesInRadius_Current + 3)
		{
			MousePosition = mousePosition;
		}
		else
		{
			if ((NumberOfEnemiesInRadius_Current >= NumberOfEnemiesInRadius_Start && NumberOfEnemiesInRadius_Start > 0) || (NumberOfEnemiesInRadius_Start == 2 && NumberOfEnemiesInRadius_Current == 1 && playerData.instance.MonstersLevel < 3) || (NumberOfEnemiesInRadius_Start == 3 && NumberOfEnemiesInRadius_Current == 2) || (NumberOfEnemiesInRadius_Start == 4 && NumberOfEnemiesInRadius_Current >= 2) || (NumberOfEnemiesInRadius_Start == 5 && NumberOfEnemiesInRadius_Current >= 3) || (NumberOfEnemiesInRadius_Start == 6 && NumberOfEnemiesInRadius_Current >= 3) || (NumberOfEnemiesInRadius_Start == 7 && NumberOfEnemiesInRadius_Current >= 4) || NumberOfEnemiesInRadius_Current >= 5)
			{
				return;
			}
			MousePosition = mousePosition;
		}
		NumberOfEnemiesInRadius_Start = num;
		NumberOfEnemiesInRadius_Current = num;
	}

	private void MoveAround()
	{
		NumberOfEnemiesInRadius_Current = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(MousePosition, radiusToCheck).Count;
		List<Vector2> list = new List<Vector2>();
		int num = 10;
		for (int i = 0; i < num; i++)
		{
			float f = UnityEngine.Random.Range(0f, MathF.PI * 2f);
			float num2 = UnityEngine.Random.Range(0f, radiusToCheck);
			Vector2 item = MousePosition + new Vector2(Mathf.Cos(f) * num2, Mathf.Sin(f) * num2);
			list.Add(item);
		}
		foreach (Vector2 item2 in list)
		{
			int count = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(item2, radiusToCheck).Count;
			if (count > NumberOfEnemiesInRadius_Current)
			{
				MousePosition = item2;
				NumberOfEnemiesInRadius_Current = count;
			}
		}
	}

	private (int numberOfEnemies, Vector2 Position) GetGoodMousePosition()
	{
		if (isThereisBountyAvailable)
		{
			NumberOfEnemiesInRadius_Start = 0;
			NumberOfEnemiesInRadius_Current = 0;
			return (numberOfEnemies: 0, Position: BountyPosition);
		}
		if (isThereisTowerAvailable)
		{
			NumberOfEnemiesInRadius_Start = 0;
			NumberOfEnemiesInRadius_Current = 0;
			return (numberOfEnemies: 0, Position: TowerPosition);
		}
		List<Vector2> list = EnemiesManager.instance.SampleEvenlySpacedPointsInTrapezoid(100);
		list.AddRange(EnemiesManager.instance.AliveEnemies_Transform.Values.Select((Transform x) => new Vector2(x.position.x, x.position.y)));
		Vector2 item = Vector2.zero;
		int num = 0;
		foreach (Vector2 item2 in list)
		{
			int count = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(item2, radiusToCheck).Count;
			if (count > num)
			{
				num = count;
				item = item2;
			}
		}
		if (num <= NumberOfEnemiesInRadius_Current)
		{
			return (numberOfEnemies: NumberOfEnemiesInRadius_Current, Position: MousePosition);
		}
		return (numberOfEnemies: num, Position: item);
	}

	private void Update()
	{
		if (!isActivateBot || Input.GetKey(KeyCode.Space) || !isRunStarted)
		{
			return;
		}
		if (!isDeciding)
		{
			MoveAroundCheckTimer += Time.deltaTime;
			if (MoveAroundCheckTimer >= MoveAroundCheckEvery)
			{
				MoveAroundCheckTimer = 0f;
				MoveAround();
			}
		}
		Vector3 position = new Vector3(MousePosition.x, MousePosition.y, 0f);
		Vector3 vector = Camera.main.WorldToScreenPoint(position);
		Mouse.current.WarpCursorPosition(vector);
	}
}
public class FinishManager : MonoBehaviour
{
	public static FinishManager instance;

	[Header("UI")]
	public GameObject FinishGamePanelGO;

	public Image FinishGamePanel;

	public TextMeshProUGUI FinishGameText;

	[Header("Typewriter")]
	[Range(0.001f, 0.2f)]
	public float baseCharDelay = 0.035f;

	[Range(1f, 10f)]
	public int tickEveryNChars = 3;

	public float commaPause = 0.08f;

	public float periodPause = 0.2f;

	public float newlinePause = 0.1f;

	[Header("Cinematic FX")]
	public float sentencePunch = 0.1f;

	public float sentencePunchTime = 0.18f;

	public float glowPulseStrength = 0.6f;

	public float glowPulseTime = 1.25f;

	private DG.Tweening.Sequence panelSeq;

	private Coroutine typewriterCR;

	private bool isTyping;

	private bool skipRequested;

	public GameObject SteamButtonGO;

	public GameObject MenuButtonGO;

	public GameObject QuitButtonGO;

	private int jumpCounter;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
	}

	public void PlayEnding_Function()
	{
		typewriterCR = StartCoroutine(PlayEnding());
	}

	public void GoToMainMenu()
	{
		FinishGamePanelGO.SetActive(value: false);
		UIManager.instance.ClickedOnGoToMainMenu();
	}

	private IEnumerator PlayEnding()
	{
		isTyping = true;
		skipRequested = false;
		FinishGamePanelGO.SetActive(value: true);
		FinishGameText.text = "";
		FinishGameText.maxVisibleCharacters = 0;
		Color color = FinishGamePanel.color;
		color.a = 0f;
		FinishGamePanel.color = color;
		panelSeq?.Kill();
		panelSeq = DOTween.Sequence().Append(FinishGamePanel.DOFade(1f, 3.6f).SetEase(Ease.InOutSine)).Join(FinishGamePanel.rectTransform.DOScale(1.02f, 1.6f).From(1f).SetEase(Ease.OutCubic));
		yield return new WaitForSeconds(1.25f);
		int num = playerData.instance.TotalItemsDropped_FullGame.Values.Sum((int v) => v);
		(int, int, int) hourMinutesSeconds = GetHourMinutesSeconds(playerData.instance.SecondsPlayed_FullGame);
		int item = hourMinutesSeconds.Item1;
		int item2 = hourMinutesSeconds.Item2;
		int item3 = hourMinutesSeconds.Item3;
		string text = "";
		foreach (Currencies value in Enum.GetValues(typeof(Currencies)))
		{
			text += ((value != Currencies.Gold) ? "  " : " ");
			text = text + "<sprite name=" + value.ToString() + ">" + playerData.instance.TotalCurrenciesGained_FullGame[value].ToReadable();
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("#VALUE1#", playerData.instance.TotalMonstersKilled_FullGame.ToString());
		dictionary.Add("#VALUE2#", playerData.instance.TotalMouseAttacks_FullGame.ToString());
		dictionary.Add("#VALUE3#", text);
		dictionary.Add("#VALUE4#", num.ToString());
		dictionary.Add("#VALUE5#", playerData.instance.TotalRunsPlayed_FullGame.ToString());
		dictionary.Add("#VALUE6#", item.ToString());
		dictionary.Add("#VALUE7#", item2.ToString());
		dictionary.Add("#VALUE8#", item3.ToString());
		string translatedThenReplaceValues = LocalizerManager.GetTranslatedThenReplaceValues("FinishGame_Text", dictionary);
		yield return TypewriterReveal(FinishGameText, translatedThenReplaceValues);
		isTyping = false;
	}

	private IEnumerator TypewriterReveal(TextMeshProUGUI tmp, string fullText)
	{
		tmp.text = fullText;
		tmp.ForceMeshUpdate();
		int total = tmp.textInfo.characterCount;
		tmp.maxVisibleCharacters = 0;
		int visibleCount = 0;
		int soundCounter = 0;
		AnimationCurve ramp = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.85f);
		while (visibleCount < total)
		{
			if (skipRequested)
			{
				tmp.maxVisibleCharacters = total;
				SentencePunch(tmp.rectTransform);
				yield break;
			}
			char character = tmp.textInfo.characterInfo[visibleCount].character;
			visibleCount = (tmp.maxVisibleCharacters = visibleCount + 1);
			if (!char.IsWhiteSpace(character))
			{
				if (soundCounter % tickEveryNChars == 0)
				{
					FXManager.instance.PlayGeneralSound(GeneralSounds.stat_winlose_tick);
				}
				soundCounter++;
			}
			float num2 = baseCharDelay;
			switch (character)
			{
			case '!':
			case '.':
			case '?':
				num2 += periodPause;
				if (character == '!')
				{
					SentencePunch(tmp.rectTransform);
				}
				break;
			case ',':
			case ':':
			case ';':
				num2 += commaPause;
				break;
			case '\n':
				FXManager.instance.PlayGeneralSound(GeneralSounds.space_enter_tick);
				num2 += newlinePause;
				break;
			}
			float time = (float)visibleCount / (float)Mathf.Max(1, total);
			float num3 = ramp.Evaluate(time);
			float seconds = num2 * num3;
			yield return new WaitForSeconds(seconds);
		}
		yield return new WaitForSeconds(0.7f);
		SteamButtonGO.SetActive(value: true);
		yield return new WaitForSeconds(0.7f);
		MenuButtonGO.SetActive(value: true);
		QuitButtonGO.SetActive(value: true);
	}

	private void SentencePunch(RectTransform rt)
	{
		rt.DOKill(complete: true);
		rt.DOPunchScale(Vector3.one * sentencePunch, sentencePunchTime, 6, 0.9f);
		jumpCounter++;
		if (jumpCounter == 2)
		{
			FXManager.instance.PlayGeneralSound(GeneralSounds.card_activation);
		}
	}

	private (int, int, int) GetHourMinutesSeconds(float totalSeconds)
	{
		int item = Mathf.FloorToInt(totalSeconds / 3600f);
		int item2 = Mathf.FloorToInt(totalSeconds % 3600f / 60f);
		int item3 = Mathf.FloorToInt(totalSeconds % 60f);
		return (item, item2, item3);
	}
}
public class RunManager : MonoBehaviour
{
	public static RunManager instance;

	private float CurrentTimer;

	private bool isShotFired;

	[HideInInspector]
	public float TimeSpentInRun;

	public BarSelfer TimerBar;

	[HideInInspector]
	public bool isRunStarted;

	public CharacterSelfer TheCharacter;

	[HideInInspector]
	public bool IsBossRun;

	public SpriteRenderer Battle_BG_SpriteRenderer;

	public SpriteRenderer Battle_Walls_SpriteRenderer;

	public Sprite NormalRun_BG_Sprite;

	public Sprite NormalRun_Walls_Sprite;

	public Sprite BossRun_BG_Sprite;

	public Sprite BossRun_Walls_Sprite;

	public GameObject BossRun_Particles;

	private bool isJustFinishedTheGame;

	[HideInInspector]
	public bool isDoubleGoldRun;

	[HideInInspector]
	public bool isDoubleLootRun;

	private float TotalEnemiesDeathTimerIncrease;

	private Dictionary<ShinyRarity, string> ShinyRarityColors = new Dictionary<ShinyRarity, string>
	{
		{
			ShinyRarity.Normal,
			"#ffffff"
		},
		{
			ShinyRarity.Rare,
			"#f5e29f"
		},
		{
			ShinyRarity.Epic,
			"#c48b8b"
		}
	};

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		IsBossRun = false;
		isJustFinishedTheGame = false;
	}

	public void StartRun()
	{
		isJustFinishedTheGame = false;
		TimeSpentInRun = 0f;
		isDoubleGoldRun = false;
		isDoubleLootRun = false;
		TotalEnemiesDeathTimerIncrease = 0f;
		if (FunctionsNeeded.IsHappened(playerData.instance.stats.Well_ChanceForDoubleGold.Total.RealValue))
		{
			isDoubleGoldRun = true;
		}
		if (FunctionsNeeded.IsHappened(playerData.instance.stats.Well_ChanceForDoubleLoot.Total.RealValue))
		{
			isDoubleLootRun = true;
		}
		if (IsBossRun)
		{
			Battle_BG_SpriteRenderer.sprite = BossRun_BG_Sprite;
			Battle_Walls_SpriteRenderer.sprite = BossRun_Walls_Sprite;
			BossRun_Particles.SetActive(value: true);
		}
		else
		{
			Battle_BG_SpriteRenderer.sprite = NormalRun_BG_Sprite;
			Battle_Walls_SpriteRenderer.sprite = NormalRun_Walls_Sprite;
			BossRun_Particles.SetActive(value: false);
		}
		foreach (Currencies value in Enum.GetValues(typeof(Currencies)))
		{
			playerData.instance.TotalCurrenciesGained_CurrentRun[value] = 0.0;
		}
		foreach (Rarity value2 in Enum.GetValues(typeof(Rarity)))
		{
			playerData.instance.TotalItemsDropped_CurrentRun[value2] = 0;
		}
		foreach (string key in playerData.instance.ShinyCounts.Keys)
		{
			playerData.instance.TotalShinyFound_CurrentRun[key] = 0;
		}
		playerData.instance.TotalMonstersKilled_CurrentRun = 0;
		playerData.instance.TotalBountiesFound_CurrentRun = 0;
		playerData.instance.TotalAreaMarksApplied_CurrentRun = 0;
		isRunStarted = true;
		WellManager.instance.StartRun();
		PlayerManager.instance.StartRun();
		EnemiesManager.instance.StartRun();
		MouseAttacker.instance.StartRun();
		SkillsManager.instance.StartRun();
		SkillBarsManager.instance.StartRun();
		GroundClickableManager.instance.StartRun();
		GroundEffectsManager.instance.StartRun();
		UIManager.instance.ManageKillText(isStartRun: true);
		TheCharacter.StartRun();
		CurrentTimer = playerData.instance.stats.Timer.Total.RealValue;
		SettingsManager.instance.ForceHideSettings();
		DebuffManager.instance.StartRun();
		TowersManager.instance.StartRun();
		AutomatorBot.instance.StartRun();
		playerData.instance.TotalRunsPlayed_FullGame++;
	}

	public void EndRun(bool isExitedRun)
	{
		CurrentTimer = 0f;
		isRunStarted = false;
		if (playerData.instance.stats.MonstersDropPartialGold.Total.RealValue > 50)
		{
			PlayerManager.instance.GainPartialGoldOfALLNonDeadEnemies();
		}
		if (playerData.instance.MonstersLevel < DatabaseManager.MaxMonstersLevelInGame && DatabaseManager.NumberOfMonstersToUnlock[playerData.instance.MonstersLevel + 1] - playerData.instance.TotalMonstersKilled_CurrentRun <= 0)
		{
			playerData.instance.MonstersLevel++;
			UIManager.instance.ManagePlayMonsterLevelTextAndButtons();
			AzrarManager.instance.CheckAzrarUnlocks(isDelayed: false);
			PlayerManager.instance.ChangeCurrency(Currencies.ClearCurrency, 1.0);
			playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.ClearCurrency] += 1.0;
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.ClearCurrency] += 1.0;
			foreach (string key in playerData.instance.SpecialMonstersPenalty.Keys)
			{
				playerData.instance.SpecialMonstersPenalty[key] = 1f;
			}
		}
		if (!isJustFinishedTheGame)
		{
			LootCalculation();
		}
		PlayerManager.instance.EndRun();
		EnemiesManager.instance.EndRun(isExitedRun);
		MouseAttacker.instance.EndRun();
		GroundEffectsManager.instance.EndRun();
		ProjectilesManager.instance.DestroyAllProjectiles();
		SkillsManager.instance.EndRun();
		SkillBarsManager.instance.EndRun();
		GroundClickableManager.instance.EndRun();
		TheCharacter.EndRun();
		DebuffManager.instance.EndRun();
		WellManager.instance.EndRun();
		TowersManager.instance.EndRun();
		InventoryManager.instance.CheckNotifications_EndRun();
		if (playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.LevelPoints] > 0.0)
		{
			SkillsUIManager.instance.CheckNotifications_EndRun();
		}
		if (playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.GemCurrency] > 0.0)
		{
			GemsManager.instance.CheckNotifications_EndRun();
		}
		if (playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.CharacterCurrency] > 0.0)
		{
			CharacterUIManager.instance.CheckNotifications_EndRun();
		}
		if (playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.WellCurrency] > 0.0)
		{
			MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Well, isShow: true);
		}
		if (playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount && !playerData.instance.isUnlockedBosses)
		{
			MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Well, isShow: true);
		}
		if (!playerData.instance.UnlockedSystems[UnlockableSystems.Well])
		{
			WellManager.instance.ManageResetButton();
		}
		AutomatorBot.instance.EndRun();
		isDoubleGoldRun = false;
		isDoubleLootRun = false;
	}

	private void LootCalculation()
	{
		InventoryManager.instance.CheckItemDrop();
		double num = playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.Gold];
		string text = "";
		if (num > 0.0)
		{
			string text2 = ((isDoubleGoldRun && isDoubleLootRun) ? " (4x)" : ((isDoubleGoldRun || isDoubleLootRun) ? " (2x)" : ""));
			text = text + "<sprite name=Gold>" + num.ToReadable() + text2;
		}
		double num2 = playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.LevelPoints];
		if (num2 > 0.0)
		{
			text = text + "   <sprite name=LevelPoints>" + num2.ToReadable();
		}
		double num3 = playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.ClearCurrency];
		if (num3 > 0.0)
		{
			text = text + "   <sprite name=ClearCurrency>" + num3.ToReadable();
		}
		double num4 = playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.CharacterCurrency];
		if (num4 > 0.0)
		{
			string text3 = (isDoubleLootRun ? " (2x)" : "");
			text = text + "   <sprite name=CharacterCurrency>" + num4.ToReadable() + text3;
		}
		double num5 = playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.GemCurrency];
		if (num5 > 0.0)
		{
			string text4 = (isDoubleLootRun ? " (2x)" : "");
			text = text + "   <sprite name=GemCurrency>" + num5.ToReadable() + text4;
		}
		double num6 = playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.WellCurrency];
		if (num6 > 0.0)
		{
			text = text + "   <sprite name=WellCurrency>" + num6.ToReadable();
		}
		text += "<size=80%>";
		bool flag = false;
		foreach (Rarity value in Enum.GetValues(typeof(Rarity)))
		{
			int num7 = playerData.instance.TotalItemsDropped_CurrentRun[value];
			if (num7 > 0)
			{
				if (!flag)
				{
					text += "\n";
				}
				text = text + (flag ? " + " : "") + num7 + "x " + LocalizerManager.GetTranslatedValue("Item_Text") + " (" + LocalizerManager.GetTranslatedValue(value.ToString() + "_Text").ToColored(InventoryManager.instance.RarityColors_Text[(int)value]) + ")";
				flag = true;
			}
		}
		bool flag2 = false;
		foreach (string key in playerData.instance.ShinyCounts.Keys)
		{
			int num8 = playerData.instance.TotalShinyFound_CurrentRun[key];
			if (num8 > 0)
			{
				if (!flag2)
				{
					text += "\n";
				}
				text = text + (flag2 ? " + " : "") + "<color=" + ShinyRarityColors[DatabaseManager.ShinyDict[key].Rarity] + ">" + num8 + "x " + LocalizerManager.GetTranslatedValue(key + "_Name") + "</color>";
				flag2 = true;
			}
		}
		UIManager.instance.ShowEndRunPanel(text);
	}

	public void ChangeTimer(float amount)
	{
		CurrentTimer += amount;
		if (CurrentTimer >= playerData.instance.stats.Timer.Total.RealValue)
		{
			CurrentTimer = playerData.instance.stats.Timer.Total.RealValue;
		}
	}

	public void ChangeTimer_EnemyDeath(float amount)
	{
		if (!(TotalEnemiesDeathTimerIncrease > DatabaseManager.MaxTimerOnMonsterDeath))
		{
			ChangeTimer(amount);
			TotalEnemiesDeathTimerIncrease += amount;
		}
	}

	public void UpdateTimer()
	{
		CurrentTimer -= Time.deltaTime;
		if (CurrentTimer >= playerData.instance.stats.Timer.Total.RealValue)
		{
			CurrentTimer = playerData.instance.stats.Timer.Total.RealValue;
		}
		else if (CurrentTimer <= 0f)
		{
			CurrentTimer = 0f;
		}
		TimerBar.ManageBar(CurrentTimer, playerData.instance.stats.Timer.Total.RealValue);
		if (CurrentTimer <= 0f && !MouseAttacker.instance.IsForgiveTimer())
		{
			EndRun(isExitedRun: false);
		}
	}

	public void FinishTheGame()
	{
		isJustFinishedTheGame = true;
		playerData.instance.isFinishedTheGame = true;
		EndRun(isExitedRun: false);
		StartCoroutine(DelayFinishTheGame());
	}

	private IEnumerator DelayFinishTheGame()
	{
		yield return new WaitForSeconds(0.8f);
		FXManager.instance.PlayGeneralSound(GeneralSounds.ascend_appear);
		yield return new WaitForSeconds(0.2f);
		EnemiesManager.instance.KillAllEnemies_ForFinishGame();
		FinishManager.instance.PlayEnding_Function();
		AchievementsManager.instance.UnlockAchievement("DefeatTheGame");
	}

	public void Update()
	{
		playerData.instance.SecondsPlayed_FullGame += Time.deltaTime;
		if (isRunStarted)
		{
			UpdateTimer();
			TimeSpentInRun += Time.deltaTime;
		}
	}
}
public enum PerformanceStats
{
	ReactionTime,
	Accuracy,
	TargetHits
}
[CreateAssetMenu]
public class WellInfo : ScriptableObject
{
	[HideInInspector]
	public string FunctionName;

	public Sprite Icon;

	public int Order;

	public List<double> MainStatValue;

	public List<StatInfo> MainStat;

	public List<double> ExtraStatValue;

	public List<StatInfo> ExtraStat;
}
public class WellManager : SerializedMonoBehaviour
{
	public static WellManager instance;

	public Transform WellTopCenter;

	public Transform WellTransform;

	public GameObject WellGO;

	public BarSelfer WellBar;

	public TextMeshProUGUI FillCountText;

	public Dictionary<int, GameObject> WellStatsGos = new Dictionary<int, GameObject>();

	private Dictionary<int, Image> WellStatsIcons = new Dictionary<int, Image>();

	private Dictionary<int, Image> WellStatsBGs = new Dictionary<int, Image>();

	private Dictionary<string, GameObject> WellStatsHighlighterGos = new Dictionary<string, GameObject>();

	public Image DescBGImage;

	public Image DescNameImage;

	public Image DescButtonImage;

	public TextMeshProUGUI DescText;

	public TextMeshProUGUI NameText;

	public TextMeshProUGUI ButtonText;

	public GameObject NameGO;

	public GameObject ButtonGO;

	private Button UpgradeButton;

	public Button ResetButton;

	public TextMeshProUGUI ResetButtonText;

	public Color StatBG_Level0Color;

	public Color StatBG_Level1Color;

	public Color StatBG_Level2Color;

	private List<Color> StatBGColors = new List<Color>();

	public Color StatIcon_Level0Color;

	public Color StatIcon_Level1Color;

	public Color StatIcon_Level2Color;

	private List<Color> StatIconColors = new List<Color>();

	public Color DescBG_Level1Color;

	public Color DescName_Level1Color;

	public Color DescBG_RedColor;

	public Color DescName_RedColor;

	public Color DescButton_Level1Color;

	public Color DescButton_RedColor;

	public Color Name_Level1Color;

	public Color Name_RedColor;

	public Color DescText_Level1Color;

	public Color DescText_RedColor;

	private Vector2 CenterLocalXPosForDesc = new Vector2(0f, 0f);

	private Vector2 LeftLocalXPosForDesc = new Vector2(-80f, -3f);

	private Vector2 CenterLocalXPosForButton = new Vector2(0f, -13f);

	private Vector2 RightLocalXPosForButton = new Vector2(287f, -5f);

	private string CurrentClickedWellPower = "";

	private int CurrentClickedWellPower_Order = -1;

	[HideInInspector]
	public bool isWellInRun;

	private double ResetCostMultiplier = 15.0;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		StatBGColors.Add(StatBG_Level0Color);
		StatBGColors.Add(StatBG_Level1Color);
		StatBGColors.Add(StatBG_Level2Color);
		StatIconColors.Add(StatIcon_Level0Color);
		StatIconColors.Add(StatIcon_Level1Color);
		StatIconColors.Add(StatIcon_Level2Color);
		UpgradeButton = ButtonGO.GetComponent<Button>();
		foreach (WellInfo wellInfo in DatabaseManager.WellList)
		{
			WellStatsIcons.Add(wellInfo.Order, WellStatsGos[wellInfo.Order].transform.Find("Icon").GetComponent<Image>());
			WellStatsBGs.Add(wellInfo.Order, WellStatsGos[wellInfo.Order].transform.Find("BG").GetComponent<Image>());
			WellStatsHighlighterGos.Add(wellInfo.FunctionName, WellStatsGos[wellInfo.Order].transform.Find("Highlighter").gameObject);
			WellStatsHighlighterGos[wellInfo.FunctionName].SetActive(value: false);
			WellStatsGos[wellInfo.Order].GetComponent<Button>().onClick.AddListener(delegate
			{
				ClickedOnWellPower(wellInfo.Order);
			});
			UpdateWellPower(wellInfo.Order);
		}
		WellStatsGos[8].GetComponent<Button>().onClick.AddListener(delegate
		{
			ClickedOnWellPower(8);
		});
		WellStatsHighlighterGos.Add("BossUnlock", WellStatsGos[8].transform.Find("Highlighter").gameObject);
		WellStatsHighlighterGos["BossUnlock"].SetActive(value: false);
		UpdateDesc(-1);
		Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.WellCurrency] = (Action)Delegate.Combine(onCurrencyChange[Currencies.WellCurrency], new Action(ManageUpgradeButton));
		onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.Gold] = (Action)Delegate.Combine(onCurrencyChange[Currencies.Gold], new Action(ManageResetButton));
		ManageBarAndFillCount();
		UpdateResetCost();
		ManageResetButton();
		CheckWellInRun();
	}

	private void CheckWellInRun()
	{
		WellGO.SetActive(value: false);
		isWellInRun = false;
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Well] && playerData.instance.WellFillCount < DatabaseManager.MaxWellFillCount)
		{
			WellGO.SetActive(value: true);
			isWellInRun = true;
		}
	}

	public void StartRun()
	{
		CheckWellInRun();
	}

	public void EndRun()
	{
		ManageBarAndFillCount();
		UpdateDesc(CurrentClickedWellPower_Order);
		if (playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount)
		{
			WellGO.SetActive(value: false);
		}
	}

	public void UnlockedWellSystem()
	{
		WellGO.SetActive(value: true);
		UpdateResetCost();
	}

	private void UpdateWellPower(int order)
	{
		string key = DatabaseManager.WellList.FirstOrDefault((WellInfo x) => x.Order == order)?.FunctionName;
		WellStatsIcons[order].sprite = DatabaseManager.WellDict[key].Icon;
		WellStatsIcons[order].color = StatIconColors[playerData.instance.WellPowerLevels[key]];
		WellStatsBGs[order].color = StatBGColors[playerData.instance.WellPowerLevels[key]];
		FunctionsNeeded.ConstrainImageSize(WellStatsIcons[order].rectTransform, WellStatsIcons[order], 70f, 70f);
	}

	public void ClickedOnWellPower(int order)
	{
		FXManager.instance.PlayUIClickSound();
		UpdateDesc(order);
	}

	private void UpdateDesc(int order)
	{
		if (!string.IsNullOrEmpty(CurrentClickedWellPower))
		{
			WellStatsHighlighterGos[CurrentClickedWellPower].SetActive(value: false);
		}
		CurrentClickedWellPower_Order = order;
		if (order == -1)
		{
			CurrentClickedWellPower = "";
			NameGO.SetActive(value: false);
			ButtonGO.SetActive(value: false);
			DescBGImage.color = DescBG_Level1Color;
			DescText.transform.localPosition = new Vector3(CenterLocalXPosForDesc.x, CenterLocalXPosForDesc.y, DescText.transform.localPosition.z);
			DescText.text = LocalizerManager.GetTranslatedValue("ClickOnAnyWellPower_Text");
			DescText.alignment = TextAlignmentOptions.Center;
			DescText.color = DescText_Level1Color;
		}
		else if (order < 8)
		{
			ButtonGO.SetActive(value: true);
			NameGO.SetActive(value: true);
			CurrentClickedWellPower = DatabaseManager.WellList.FirstOrDefault((WellInfo x) => x.Order == order)?.FunctionName;
			WellStatsHighlighterGos[CurrentClickedWellPower].SetActive(value: true);
			WellInfo wellInfo = DatabaseManager.WellDict[CurrentClickedWellPower];
			ButtonGO.transform.localPosition = new Vector3(RightLocalXPosForButton.x, RightLocalXPosForButton.y, ButtonGO.transform.localPosition.z);
			int num = playerData.instance.WellPowerLevels[CurrentClickedWellPower];
			DescBGImage.color = DescBG_Level1Color;
			DescNameImage.color = DescName_Level1Color;
			DescButtonImage.color = DescButton_Level1Color;
			DescText.transform.localPosition = new Vector3(LeftLocalXPosForDesc.x, LeftLocalXPosForDesc.y, DescText.transform.localPosition.z);
			DescText.alignment = TextAlignmentOptions.Left;
			NameText.text = LocalizerManager.GetTranslatedValue("WellName_" + DatabaseManager.WellDict[CurrentClickedWellPower].FunctionName);
			DescText.color = DescText_Level1Color;
			NameText.color = Name_Level1Color;
			string text = "";
			for (int i = 0; i < wellInfo.MainStat.Count; i++)
			{
				StatInfo statInfo = wellInfo.MainStat[i];
				text += statInfo.GetValueDescText_SingleOrMultipleValues(new List<double> { wellInfo.MainStatValue[i] }, isColoredTag: false);
				if (i < wellInfo.MainStat.Count - 1)
				{
					text += "\n";
				}
			}
			string text2 = "";
			for (int j = 0; j < wellInfo.ExtraStat.Count; j++)
			{
				StatInfo statInfo2 = wellInfo.ExtraStat[j];
				text2 += statInfo2.GetValueDescText_SingleOrMultipleValues(new List<double> { wellInfo.ExtraStatValue[j] }, isColoredTag: false);
				if (j < wellInfo.ExtraStat.Count - 1)
				{
					text2 += "\n";
				}
			}
			switch (num)
			{
			case 0:
				ButtonText.text = LocalizerManager.GetTranslatedValue("Unlock_Text") + "\n<sprite name=WellCurrency>1";
				DescText.text = text;
				break;
			case 1:
			{
				ButtonText.text = LocalizerManager.GetTranslatedValue("Empower_Text") + "\n<sprite name=WellCurrency>2";
				string text3 = LocalizerManager.GetTranslatedValue("Empowered_Text") + ": <alpha=88f>" + text2;
				DescText.text = text + "\n" + text3;
				break;
			}
			default:
				ButtonText.text = LocalizerManager.GetTranslatedValue("Empowered_Text");
				DescText.text = text + "\n" + text2;
				break;
			}
		}
		else
		{
			CurrentClickedWellPower = "BossUnlock";
			WellStatsHighlighterGos[CurrentClickedWellPower].SetActive(value: true);
			NameGO.SetActive(value: true);
			DescBGImage.color = DescBG_RedColor;
			DescNameImage.color = DescName_RedColor;
			DescButtonImage.color = DescButton_RedColor;
			DescText.color = DescText_RedColor;
			NameText.color = Name_RedColor;
			if (playerData.instance.WellFillCount < DatabaseManager.MaxWellFillCount)
			{
				ButtonGO.SetActive(value: false);
				DescText.text = LocalizerManager.GetTranslatedThenReplaceValues("FillTheWellXTimes_Text", DatabaseManager.MaxWellFillCount.ToString());
				NameText.text = "? ? ?";
				DescText.transform.localPosition = new Vector3(CenterLocalXPosForDesc.x, LeftLocalXPosForDesc.y, DescText.transform.localPosition.z);
				DescText.alignment = TextAlignmentOptions.Center;
			}
			else if (playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount && !playerData.instance.isUnlockedBosses)
			{
				NameText.text = "? ? ?";
				ButtonText.text = "? ? ?";
				ButtonGO.transform.localPosition = new Vector3(CenterLocalXPosForButton.x, CenterLocalXPosForButton.y, ButtonGO.transform.localPosition.z);
				ButtonGO.SetActive(value: true);
				DescText.text = "";
				DescText.transform.localPosition = new Vector3(CenterLocalXPosForDesc.x, LeftLocalXPosForDesc.y, DescText.transform.localPosition.z);
				DescText.alignment = TextAlignmentOptions.Center;
			}
			else
			{
				ButtonGO.SetActive(value: true);
				NameText.text = LocalizerManager.GetTranslatedValue("BossUnlock_Name");
				DescText.text = LocalizerManager.GetTranslatedValue("BossUnlock_Desc");
				ButtonText.text = LocalizerManager.GetTranslatedValue("BossUnlock_Button");
				DescText.transform.localPosition = new Vector3(LeftLocalXPosForDesc.x, LeftLocalXPosForDesc.y, DescText.transform.localPosition.z);
				DescText.alignment = TextAlignmentOptions.Left;
				ButtonGO.transform.localPosition = new Vector3(RightLocalXPosForButton.x, RightLocalXPosForButton.y, ButtonGO.transform.localPosition.z);
			}
		}
		ManageUpgradeButton();
	}

	public void ManageUpgradeButton()
	{
		if (string.IsNullOrEmpty(CurrentClickedWellPower))
		{
			return;
		}
		if (CurrentClickedWellPower == "BossUnlock")
		{
			UpgradeButton.interactable = true;
			return;
		}
		switch (playerData.instance.WellPowerLevels[CurrentClickedWellPower])
		{
		case 0:
			UpgradeButton.interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.WellCurrency, 1.0, IsSpendAlso: false);
			break;
		case 1:
			UpgradeButton.interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.WellCurrency, 2.0, IsSpendAlso: false);
			break;
		default:
			UpgradeButton.interactable = false;
			break;
		}
	}

	public void ClickedOnUpgrade()
	{
		FXManager.instance.PlayUIClickSound();
		if (CurrentClickedWellPower == "BossUnlock")
		{
			playerData.instance.isUnlockedBosses = true;
			RunManager.instance.IsBossRun = true;
			UIManager.instance.ClickedOnStartRun();
			return;
		}
		int num = ((playerData.instance.WellPowerLevels[CurrentClickedWellPower] == 0) ? 1 : 2);
		if (PlayerManager.instance.IsCanSpendCurrency(Currencies.WellCurrency, num, IsSpendAlso: true))
		{
			FXManager.instance.PlayGeneralSound(GeneralSounds.purchase);
			int order = DatabaseManager.WellDict[CurrentClickedWellPower].Order;
			playerData.instance.WellPowerLevels[CurrentClickedWellPower]++;
			UpdateWellPower(order);
			UpdateDesc(order);
			ApplyRemoveWellPower(CurrentClickedWellPower, isApply: true);
			ManageUpgradeButton();
		}
	}

	private void ApplyRemoveWellPower(string wellPowerName, bool isApply)
	{
		int num = playerData.instance.WellPowerLevels[wellPowerName];
		WellInfo wellInfo = DatabaseManager.WellDict[wellPowerName];
		switch (num)
		{
		case 1:
		{
			for (int k = 0; k < wellInfo.MainStat.Count; k++)
			{
				StatInfo statInfo3 = wellInfo.MainStat[k];
				playerData.instance.stats.ChangeAStat(statInfo3.VariableName, statInfo3.StatsProp, wellInfo.MainStatValue[k], isApply);
			}
			break;
		}
		case 2:
		{
			for (int i = 0; i < wellInfo.ExtraStat.Count; i++)
			{
				StatInfo statInfo = wellInfo.ExtraStat[i];
				playerData.instance.stats.ChangeAStat(statInfo.VariableName, statInfo.StatsProp, wellInfo.ExtraStatValue[i], isApply);
			}
			if (!isApply)
			{
				for (int j = 0; j < wellInfo.MainStat.Count; j++)
				{
					StatInfo statInfo2 = wellInfo.MainStat[j];
					playerData.instance.stats.ChangeAStat(statInfo2.VariableName, statInfo2.StatsProp, wellInfo.MainStatValue[j], isApply);
				}
			}
			break;
		}
		}
	}

	public void ManageBarAndFillCount()
	{
		FillCountText.text = playerData.instance.WellFillCount + " / " + DatabaseManager.MaxWellFillCount;
		if (playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount)
		{
			WellBar.ManageBar(1.0, 1.0);
			WellBar.BarText.text = LocalizerManager.GetTranslatedValue("Max_Text");
		}
		else
		{
			WellBar.ManageBar(playerData.instance.WellCurrentExp, DatabaseManager.Well_ExpToLevelUp(playerData.instance.WellFillCount + 1));
		}
	}

	public void MonsterDiedGainExp()
	{
		if (!playerData.instance.UnlockedSystems[UnlockableSystems.Well] || playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount)
		{
			return;
		}
		int monstersLevel = playerData.instance.MonstersLevel;
		double num = playerData.instance.WellCurrentExp / DatabaseManager.Well_ExpToLevelUp(playerData.instance.WellFillCount + 1);
		if (monstersLevel < DatabaseManager.MaxMonstersLevelInGame && playerData.instance.WellFillCount == DatabaseManager.MaxWellFillCount - 1 && num >= 0.95)
		{
			return;
		}
		playerData.instance.WellCurrentExp += DatabaseManager.Well_EnemyExpDrop(monstersLevel);
		while (playerData.instance.WellCurrentExp >= DatabaseManager.Well_ExpToLevelUp(playerData.instance.WellFillCount + 1))
		{
			playerData.instance.WellCurrentExp -= DatabaseManager.Well_ExpToLevelUp(playerData.instance.WellFillCount + 1);
			playerData.instance.WellFillCount++;
			PlayerManager.instance.ChangeCurrency(Currencies.WellCurrency, 1.0);
			playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.WellCurrency] += 1.0;
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.WellCurrency] += 1.0;
			MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Well, isShow: true);
			AchievementsManager.instance.UnlockAchievement("WellFill");
			if (playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount)
			{
				UIManager.instance.PopText_WellManager();
				break;
			}
		}
		ManageBarAndFillCount();
	}

	public void GainWellCurrency_Azrar()
	{
		PlayerManager.instance.ChangeCurrency(Currencies.WellCurrency, 1.0);
		playerData.instance.TotalCurrenciesGained_FullGame[Currencies.WellCurrency] += 1.0;
		MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Well, isShow: true);
	}

	public void UpdateResetCost()
	{
		int num = Mathf.Min(playerData.instance.MonsterLevelWhenWellReset, DatabaseManager.MaxMonstersLevelInGame);
		double number = ResetCostMultiplier * (double)DatabaseManager.NumberOfMonstersToUnlock[num] * DatabaseManager.EnemyGold(num, isIgnoreBossRun: true);
		ResetButtonText.text = LocalizerManager.GetTranslatedValue("ResetWellPowers_Text") + "\n<sprite name=Gold>" + number.ToReadable();
		ManageResetButton();
	}

	public void ManageResetButton()
	{
		int num = Mathf.Min(playerData.instance.MonsterLevelWhenWellReset, DatabaseManager.MaxMonstersLevelInGame);
		double amount = ResetCostMultiplier * (double)DatabaseManager.NumberOfMonstersToUnlock[num] * DatabaseManager.EnemyGold(num, isIgnoreBossRun: true);
		ResetButton.interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.Gold, amount, IsSpendAlso: false);
	}

	public void ClickedOnReset()
	{
		int num = Mathf.Min(playerData.instance.MonsterLevelWhenWellReset, DatabaseManager.MaxMonstersLevelInGame);
		double amount = ResetCostMultiplier * (double)DatabaseManager.NumberOfMonstersToUnlock[num] * DatabaseManager.EnemyGold(num, isIgnoreBossRun: true);
		if (!PlayerManager.instance.IsCanSpendCurrency(Currencies.Gold, amount, IsSpendAlso: true))
		{
			return;
		}
		FXManager.instance.PlayGeneralSound(GeneralSounds.purchase);
		playerData.instance.MonsterLevelWhenWellReset = playerData.instance.MonstersLevel;
		UpdateResetCost();
		foreach (WellInfo well in DatabaseManager.WellList)
		{
			ApplyRemoveWellPower(well.FunctionName, isApply: false);
			playerData.instance.WellPowerLevels[well.FunctionName] = 0;
			UpdateWellPower(well.Order);
		}
		if (DatabaseManager.WellDict.ContainsKey(CurrentClickedWellPower))
		{
			UpdateDesc(DatabaseManager.WellDict[CurrentClickedWellPower].Order);
		}
		playerData.instance.WellCurrency = 0;
		PlayerManager.instance.ChangeCurrency(Currencies.WellCurrency, playerData.instance.WellFillCount + playerData.instance.AzrarLevels["GainWellCurrency"], isApplyGainMulti: false, isApplyAch: false);
	}

	public void FireTrailEffect(Vector3 startPos)
	{
		if (isWellInRun)
		{
			bool IsInstatiated;
			GameObject gameObject = ObjectPooler.instance.GiveMeObject("WellTrail", base.transform, startPos, out IsInstatiated);
			if (IsInstatiated)
			{
				gameObject.GetComponent<PSSelfer>().TakeInfo("WellTrail", 2f);
			}
			Vector3 targetPos = WellTopCenter.position + UnityEngine.Random.insideUnitSphere * 30f;
			gameObject.GetComponent<TrailAddMultiController>().TakeInfo(startPos, targetPos);
		}
	}

	public void BounceWell()
	{
		WellTransform.DOKill(complete: true);
		WellTransform.DOPunchScale(UnityEngine.Random.Range(0.02f, 0.03f) * Vector3.one, UnityEngine.Random.Range(0.15f, 0.25f));
	}
}
public class GemsManager : MonoBehaviour
{
	public static GemsManager instance;

	public Transform GemsParent;

	private Dictionary<int, GameObject> Gems_LockedGOs = new Dictionary<int, GameObject>();

	private Dictionary<int, GameObject> Gems_UnlockedGOs = new Dictionary<int, GameObject>();

	private Dictionary<int, Image> Gems_Icons = new Dictionary<int, Image>();

	private Dictionary<int, TextMeshProUGUI> Gems_LevelTexts = new Dictionary<int, TextMeshProUGUI>();

	private Dictionary<int, TextMeshProUGUI> Gems_RerollTexts = new Dictionary<int, TextMeshProUGUI>();

	private Dictionary<int, TextMeshProUGUI> Gems_UnlockTexts = new Dictionary<int, TextMeshProUGUI>();

	private Dictionary<int, Button> Gems_LevelButtons = new Dictionary<int, Button>();

	private Dictionary<int, Button> Gems_RerollButtons = new Dictionary<int, Button>();

	private Dictionary<int, Button> Gems_UnlockButtons = new Dictionary<int, Button>();

	private Dictionary<int, TextMeshProUGUI> Gems_DescText = new Dictionary<int, TextMeshProUGUI>();

	public List<Sprite> GemsIcons = new List<Sprite>();

	private Dictionary<int, List<string>> GemStatsForEveryLevel = new Dictionary<int, List<string>>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.GemCurrency] = (Action)Delegate.Combine(onCurrencyChange[Currencies.GemCurrency], new Action(ManageAllGemsButtons));
		for (int j = 0; j < GemsParent.childCount; j++)
		{
			Gems_LockedGOs.Add(j, GemsParent.GetChild(j).Find("Lock").gameObject);
			Gems_UnlockedGOs.Add(j, GemsParent.GetChild(j).Find("Unlocked").gameObject);
			Gems_Icons.Add(j, GemsParent.GetChild(j).Find("Unlocked").Find("IconPlace")
				.Find("Icon")
				.GetComponent<Image>());
			Gems_DescText.Add(j, GemsParent.GetChild(j).Find("Unlocked").Find("DescText")
				.GetComponent<TextMeshProUGUI>());
			Gems_LevelTexts.Add(j, GemsParent.GetChild(j).Find("Unlocked").Find("LevelUpButton")
				.Find("LevelText")
				.GetComponent<TextMeshProUGUI>());
			Gems_RerollTexts.Add(j, GemsParent.GetChild(j).Find("Unlocked").Find("RerollButton")
				.Find("RerollText")
				.GetComponent<TextMeshProUGUI>());
			Gems_UnlockTexts.Add(j, GemsParent.GetChild(j).Find("Lock").Find("UnlockButton")
				.Find("CostText")
				.GetComponent<TextMeshProUGUI>());
			Gems_LevelButtons.Add(j, GemsParent.GetChild(j).Find("Unlocked").Find("LevelUpButton")
				.GetComponent<Button>());
			Gems_RerollButtons.Add(j, GemsParent.GetChild(j).Find("Unlocked").Find("RerollButton")
				.GetComponent<Button>());
			Gems_UnlockButtons.Add(j, GemsParent.GetChild(j).Find("Lock").Find("UnlockButton")
				.GetComponent<Button>());
			int gemIndex = j;
			Gems_LevelButtons[gemIndex].onClick.AddListener(delegate
			{
				ClickedOnLevel(gemIndex);
			});
			Gems_RerollButtons[gemIndex].onClick.AddListener(delegate
			{
				ClickedOnReroll(gemIndex);
			});
			Gems_UnlockButtons[gemIndex].onClick.AddListener(delegate
			{
				ClickedOnUnlock(gemIndex);
			});
		}
		int i;
		for (i = 1; i <= 7; i++)
		{
			GemStatsForEveryLevel.Add(i, (from a in DatabaseManager.GemStatDict
				where i >= a.Value.AppearGemLevel && i < a.Value.MaxGemLevel_Exclusive
				select a.Key).ToList());
		}
		ManageAllGemsButtons();
		for (int k = 0; k < GemsParent.childCount; k++)
		{
			UpdateGem(k);
		}
	}

	private void ManageAllGemsButtons()
	{
		for (int i = 0; i < GemsParent.childCount; i++)
		{
			if (playerData.instance.GemsIsUnlocked[i])
			{
				int currentLevel = playerData.instance.Gems[i].CurrentLevel;
				int num = ((playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue < 50) ? 6 : 7);
				Gems_LevelButtons[i].interactable = currentLevel < num && PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, DatabaseManager.GemLevelCost[i][currentLevel + 1], IsSpendAlso: false);
				Gems_RerollButtons[i].interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, DatabaseManager.GemRerollCost, IsSpendAlso: false);
			}
			else
			{
				Gems_UnlockButtons[i].interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, DatabaseManager.GemUnlockCost[i], IsSpendAlso: false);
			}
		}
	}

	private void UpdateGem(int gemIndex)
	{
		bool flag = !playerData.instance.GemsIsUnlocked[gemIndex];
		Gems_LockedGOs[gemIndex].SetActive(flag);
		Gems_UnlockedGOs[gemIndex].SetActive(!flag);
		if (flag)
		{
			Gems_UnlockTexts[gemIndex].text = "<sprite name=GemCurrency>" + DatabaseManager.GemUnlockCost[gemIndex];
			return;
		}
		int currentLevel = playerData.instance.Gems[gemIndex].CurrentLevel;
		if (currentLevel >= 7 || (currentLevel >= 6 && playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue < 50))
		{
			Gems_LevelTexts[gemIndex].text = LocalizerManager.GetTranslatedValue("Max_Text");
		}
		else
		{
			int num = DatabaseManager.GemLevelCost[gemIndex][currentLevel + 1];
			Gems_LevelTexts[gemIndex].text = LocalizerManager.GetTranslatedValue("Lv_Text") + currentLevel + "\n<sprite name=GemCurrency>" + num;
		}
		int gemRerollCost = DatabaseManager.GemRerollCost;
		Gems_RerollTexts[gemIndex].text = LocalizerManager.GetTranslatedValue("Reroll_Text") + "\n<sprite name=GemCurrency>" + gemRerollCost;
		Gems_Icons[gemIndex].sprite = GemsIcons[playerData.instance.Gems[gemIndex].IconIndex];
		string text = "";
		for (int i = 1; i <= 7; i++)
		{
			if (i != 7 || playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue >= 50)
			{
				GemStatInfo gemStatInfo = DatabaseManager.GemStatDict[playerData.instance.Gems[gemIndex].LevelsAndStatsNames[i]];
				StatInfo stat = gemStatInfo.stat;
				string skillNameFromStatName = GetSkillNameFromStatName(stat.VariableName);
				skillNameFromStatName += stat.GetValueDescText_SingleOrMultipleValues(new List<double> { gemStatInfo.LevelsAndValues[i] }, isColoredTag: false);
				if (i == 1 || i == 2)
				{
					skillNameFromStatName = GetSkillNameFromStatName(stat.VariableName);
					skillNameFromStatName += stat.GetValueDescText_SingleOrMultipleValues(new List<double> { gemStatInfo.LevelsAndValues[i] * (double)playerData.instance.stats.Well_GemsFirstLevelsEffectMultiplier.Total.RealValue }, isColoredTag: false);
				}
				text = ((playerData.instance.Gems[gemIndex].CurrentLevel < i) ? (text + "<size=105%><color=#e3cc94><alpha=44f><b>" + LocalizerManager.GetTranslatedValue("Lv_Text") + " " + i + ":</b></color></size><alpha=44f> " + skillNameFromStatName) : (text + "<size=100%><color=#e3cc94><b>" + LocalizerManager.GetTranslatedValue("Lv_Text") + " " + i + ":</b></color></size> " + skillNameFromStatName));
				if (i < 6 || (i == 6 && playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue >= 50))
				{
					text += "\n";
				}
			}
		}
		Gems_DescText[gemIndex].text = text;
	}

	public void UnlockedTheSystem()
	{
		playerData.instance.TotalGemsLeveledUp = 1;
		AchievementsManager.instance.UnlockAchievement("Gem1");
		GenerateANewGem(0);
		UpdateGem(0);
		ManageAllGemsButtons();
	}

	public void ClickedOnLevel(int gemIndex)
	{
		int num = DatabaseManager.GemLevelCost[gemIndex][playerData.instance.Gems[gemIndex].CurrentLevel + 1];
		if (PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, num, IsSpendAlso: true))
		{
			FXManager.instance.PlayUIClickSound();
			playerData.instance.TotalGemsLeveledUp++;
			playerData.instance.Gems[gemIndex].CurrentLevel++;
			string gemStatName = playerData.instance.Gems[gemIndex].LevelsAndStatsNames[playerData.instance.Gems[gemIndex].CurrentLevel];
			ApplyRemoveStat(gemStatName, playerData.instance.Gems[gemIndex].CurrentLevel, -1.0, isApply: true);
			UpdateGem(gemIndex);
			ManageAllGemsButtons();
		}
	}

	public void ClickedOnReroll(int gemIndex)
	{
		int gemRerollCost = DatabaseManager.GemRerollCost;
		if (PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, gemRerollCost, IsSpendAlso: true))
		{
			FXManager.instance.PlayUIClickSound();
			GenerateANewGem(gemIndex);
			UpdateGem(gemIndex);
			ManageAllGemsButtons();
		}
	}

	public void ClickedOnUnlock(int gemIndex)
	{
		int num = DatabaseManager.GemUnlockCost[gemIndex];
		if (PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, num, IsSpendAlso: true))
		{
			FXManager.instance.PlayUIClickSound();
			playerData.instance.TotalGemsLeveledUp++;
			switch (gemIndex)
			{
			case 1:
				AchievementsManager.instance.UnlockAchievement("Gem2");
				break;
			case 2:
				AchievementsManager.instance.UnlockAchievement("Gem2");
				AchievementsManager.instance.UnlockAchievement("Gem3");
				break;
			}
			playerData.instance.GemsIsUnlocked[gemIndex] = true;
			GenerateANewGem(gemIndex);
			UpdateGem(gemIndex);
			ManageAllGemsButtons();
		}
	}

	public void CheckNotifications_EndRun()
	{
		for (int i = 0; i < GemsParent.childCount; i++)
		{
			if (playerData.instance.GemsIsUnlocked[i])
			{
				int currentLevel = playerData.instance.Gems[i].CurrentLevel;
				int num = ((playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue < 50) ? 6 : 7);
				if (currentLevel < num && PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, DatabaseManager.GemLevelCost[i][currentLevel + 1], IsSpendAlso: false))
				{
					MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Gems, isShow: true);
					break;
				}
			}
			else if (PlayerManager.instance.IsCanSpendCurrency(Currencies.GemCurrency, DatabaseManager.GemUnlockCost[i], IsSpendAlso: false))
			{
				MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Gems, isShow: true);
				break;
			}
		}
	}

	private string GetSkillNameFromStatName(string statName)
	{
		if (statName.Contains("LC_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("LightningChain_Name") + "</b>: ";
		}
		if (statName.Contains("Knight_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("KnightSlash_Name") + "</b>: ";
		}
		if (statName.Contains("RoA_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("RainOfArrows_Name") + "</b>: ";
		}
		if (statName.Contains("Vampire_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("VampireExplosion_Name") + "</b>: ";
		}
		return "";
	}

	private void GenerateANewGem(int gemIndex)
	{
		GemSavedData gemSavedData = playerData.instance.Gems[gemIndex];
		int num = gemSavedData?.CurrentLevel ?? 1;
		if (gemSavedData != null)
		{
			for (int i = 1; i <= num; i++)
			{
				if (i != 7 || playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue >= 50)
				{
					ApplyRemoveStat(gemSavedData.LevelsAndStatsNames[i], i, -1.0, isApply: false);
				}
			}
		}
		Dictionary<int, List<string>> dictionary = GemStatsForEveryLevel.ToDictionary((KeyValuePair<int, List<string>> entry) => entry.Key, (KeyValuePair<int, List<string>> entry) => new List<string>(entry.Value.Where((string statName) => DatabaseManager.GemStatDict[statName].Condition.IsMet()).ToList()));
		IntString intString = new IntString();
		for (int j = 1; j <= 7; j++)
		{
			intString.Add(j, dictionary[j].GetOneRandom());
			if (!DatabaseManager.GemStatDict[intString[j]].isCanAppearMultipleTimesOnSameGem)
			{
				for (int k = j + 1; k <= 7; k++)
				{
					dictionary[k].Remove(intString[j]);
				}
			}
		}
		GemSavedData value = new GemSavedData(intString, UnityEngine.Random.Range(0, GemsIcons.Count), num);
		playerData.instance.Gems[gemIndex] = value;
		UpdateGem(gemIndex);
		for (int l = 1; l <= num; l++)
		{
			if (l != 7 || playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue >= 50)
			{
				ApplyRemoveStat(intString[l], l, -1.0, isApply: true);
			}
		}
	}

	private void ApplyRemoveStat(string gemStatName, int LevelOfStat, double multiplier, bool isApply)
	{
		if (multiplier < 1.0)
		{
			multiplier = ((LevelOfStat != 1 && LevelOfStat != 2) ? 1.0 : ((double)playerData.instance.stats.Well_GemsFirstLevelsEffectMultiplier.Total.RealValue));
		}
		StatInfo stat = DatabaseManager.GemStatDict[gemStatName].stat;
		double value = DatabaseManager.GemStatDict[gemStatName].LevelsAndValues[LevelOfStat] * multiplier;
		playerData.instance.stats.ChangeAStat(stat.VariableName, stat.StatsProp, value, isApply);
	}

	public void PurchasedResetEffectOfGems(bool isAfter)
	{
		for (int i = 0; i < GemsParent.childCount; i++)
		{
			GemSavedData gemSavedData = playerData.instance.Gems[i];
			if (gemSavedData != null)
			{
				double multiplier = playerData.instance.stats.Well_GemsFirstLevelsEffectMultiplier.Total.RealValue;
				if (gemSavedData.CurrentLevel >= 1)
				{
					ApplyRemoveStat(gemSavedData.LevelsAndStatsNames[1], 1, multiplier, isAfter);
				}
				if (gemSavedData.CurrentLevel >= 2)
				{
					ApplyRemoveStat(gemSavedData.LevelsAndStatsNames[2], 2, multiplier, isAfter);
				}
				UpdateGem(i);
			}
		}
	}

	public void UnlockOrLockLevelSixForGems()
	{
		for (int i = 0; i < GemsParent.childCount; i++)
		{
			GemSavedData gemSavedData = playerData.instance.Gems[i];
			if (gemSavedData != null)
			{
				if (gemSavedData.CurrentLevel == 7)
				{
					ApplyRemoveStat(gemSavedData.LevelsAndStatsNames[7], 7, -1.0, playerData.instance.stats.Well_UnlockLevelSixForGems.Total.RealValue >= 50);
				}
				UpdateGem(i);
			}
		}
		ManageAllGemsButtons();
	}
}
[CreateAssetMenu]
public class GemStatInfo : SerializedScriptableObject
{
	[HideInInspector]
	public string functionName;

	public StatInfo stat;

	public int AppearGemLevel = 1;

	public int MaxGemLevel_Exclusive = 6;

	public bool isShowSign;

	public bool isCanAppearMultipleTimesOnSameGem;

	public Dictionary<int, double> LevelsAndValues = new Dictionary<int, double>();

	public AzrarCondition Condition = new Cond_AlwaysMet();
}
[Serializable]
public class GemSavedData
{
	public IntString LevelsAndStatsNames;

	public int IconIndex;

	public int CurrentLevel;

	public GemSavedData(IntString levelsAndStatsNames, int iconIndex, int currentLevel)
	{
		LevelsAndStatsNames = levelsAndStatsNames;
		IconIndex = iconIndex;
		CurrentLevel = currentLevel;
	}
}
public class InventoryManager : MonoBehaviour
{
	public static InventoryManager instance;

	public Transform inventoryParent;

	public Transform equipmentParent;

	private List<GameObject> inventory_Highlighters = new List<GameObject>();

	private List<Image> inventory_Icons = new List<Image>();

	private List<GameObject> inventory_IconsGOs = new List<GameObject>();

	private List<Image> inventory_SlotImages = new List<Image>();

	private List<GameObject> equipment_Highlighters = new List<GameObject>();

	private List<Image> equipment_Icons = new List<Image>();

	private List<GameObject> equipment_IconsGOs = new List<GameObject>();

	private List<GameObject> equipment_Blackout = new List<GameObject>();

	private List<GameObject> equipment_PurchaseButtonGOs = new List<GameObject>();

	private List<Button> equipment_PurchaseButtons = new List<Button>();

	private List<TextMeshProUGUI> equipment_PurchaseButtonTexts = new List<TextMeshProUGUI>();

	private List<Image> equipment_SlotImages = new List<Image>();

	public static int NumberOfInventorySlots = 32;

	public static int NumberOfEquipmentSlots = 5;

	public List<Color> RarityColors = new List<Color>();

	public List<Color> RarityColors_Text = new List<Color>();

	public Color DefaultSlotColor;

	private Dictionary<Rarity, Color> RarityColorsDict = new Dictionary<Rarity, Color>();

	private Dictionary<Rarity, Color> RarityColorsDict_Text = new Dictionary<Rarity, Color>();

	private int IDOfSelectedSlot = -1;

	private bool isInventorySelected;

	public GameObject DetailsArea_EquipUnEquipButtonGO;

	public GameObject DetailsArea_SellButtonGO;

	public GameObject DetailsArea_ModTextGO;

	public GameObject DetailsArea_ModValueRangeTextGO;

	public GameObject DetailsArea_LevelTextGO;

	public GameObject DetailsArea_RarityTextGO;

	public GameObject DetailsArea_IconGO;

	public GameObject DetailsArea_SlotImageGO;

	private Button DetailsArea_EquipUnEquipButton;

	private TextMeshProUGUI DetailsArea_EquipUnEquipButtonText;

	private TextMeshProUGUI DetailsArea_SellCostText;

	private TextMeshProUGUI DetailsArea_ModText;

	private TextMeshProUGUI DetailsArea_ModValueRangeText;

	private TextMeshProUGUI DetailsArea_LevelText;

	private TextMeshProUGUI DetailsArea_RarityText;

	private Image DetailsArea_Icon;

	private Image DetailsArea_SlotImage;

	private float maxStatsTextWidth = 420f;

	public List<Sprite> ItemsIcons_Normal = new List<Sprite>();

	public List<Sprite> ItemsIcons_Rare = new List<Sprite>();

	public List<Sprite> ItemsIcons_Epic = new List<Sprite>();

	public List<Sprite> ItemsIcons_Legendary = new List<Sprite>();

	private Dictionary<Rarity, List<Sprite>> ItemsIcons = new Dictionary<Rarity, List<Sprite>>();

	private Vector2 IconSize_Inventory = new Vector2(45f, 45f);

	private Vector2 IconSize_Equipment = new Vector2(55f, 55f);

	private Vector2 IconSize_DetailsArea = new Vector2(55f, 55f);

	public GameObject AutoSellCheckmark;

	public GameObject AutoSellTextGO;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		for (int i = 0; i < RarityColors.Count; i++)
		{
			RarityColorsDict.Add((Rarity)i, RarityColors[i]);
			RarityColorsDict_Text.Add((Rarity)i, RarityColors_Text[i]);
		}
		ItemsIcons.Add(Rarity.Normal, ItemsIcons_Normal);
		ItemsIcons.Add(Rarity.Rare, ItemsIcons_Rare);
		ItemsIcons.Add(Rarity.Epic, ItemsIcons_Epic);
		ItemsIcons.Add(Rarity.Legendary, ItemsIcons_Legendary);
		DetailsArea_EquipUnEquipButton = DetailsArea_EquipUnEquipButtonGO.GetComponent<Button>();
		DetailsArea_SellCostText = DetailsArea_SellButtonGO.GetComponentInChildren<TextMeshProUGUI>();
		DetailsArea_EquipUnEquipButtonText = DetailsArea_EquipUnEquipButtonGO.GetComponentInChildren<TextMeshProUGUI>();
		DetailsArea_ModText = DetailsArea_ModTextGO.GetComponent<TextMeshProUGUI>();
		DetailsArea_ModValueRangeText = DetailsArea_ModValueRangeTextGO.GetComponent<TextMeshProUGUI>();
		DetailsArea_LevelText = DetailsArea_LevelTextGO.GetComponent<TextMeshProUGUI>();
		DetailsArea_RarityText = DetailsArea_RarityTextGO.GetComponent<TextMeshProUGUI>();
		DetailsArea_Icon = DetailsArea_IconGO.GetComponent<Image>();
		DetailsArea_SlotImage = DetailsArea_SlotImageGO.GetComponent<Image>();
		for (int j = 0; j < inventoryParent.childCount; j++)
		{
			inventory_Highlighters.Add(inventoryParent.GetChild(j).Find("Highlighter").gameObject);
			inventory_Icons.Add(inventoryParent.GetChild(j).Find("Icon").GetComponent<Image>());
			inventory_IconsGOs.Add(inventoryParent.GetChild(j).Find("Icon").gameObject);
			inventory_SlotImages.Add(inventoryParent.GetChild(j).GetComponent<Image>());
			int number = j;
			inventoryParent.GetChild(j).GetComponent<Button>().onClick.AddListener(delegate
			{
				ClickedOnSlot(isInventory: true, number);
			});
			UpdateSlot(isInventory: true, j);
		}
		for (int k = 0; k < equipmentParent.childCount; k++)
		{
			equipment_Highlighters.Add(equipmentParent.GetChild(k).Find("Highlighter").gameObject);
			equipment_Icons.Add(equipmentParent.GetChild(k).Find("Icon").GetComponent<Image>());
			equipment_IconsGOs.Add(equipmentParent.GetChild(k).Find("Icon").gameObject);
			equipment_Blackout.Add(equipmentParent.GetChild(k).Find("Blackout").gameObject);
			equipment_PurchaseButtonGOs.Add(equipmentParent.GetChild(k).Find("PurchaseButton").gameObject);
			equipment_PurchaseButtons.Add(equipmentParent.GetChild(k).Find("PurchaseButton").GetComponent<Button>());
			equipment_PurchaseButtonTexts.Add(equipmentParent.GetChild(k).Find("PurchaseButton").Find("CostText")
				.GetComponent<TextMeshProUGUI>());
			equipment_SlotImages.Add(equipmentParent.GetChild(k).GetComponent<Image>());
			equipment_PurchaseButtonTexts[k].text = "<sprite name=Gold>" + DatabaseManager.EquipmentUnlockCost[k].ToReadable();
			int number2 = k;
			equipmentParent.GetChild(k).GetComponent<Button>().onClick.AddListener(delegate
			{
				ClickedOnSlot(isInventory: false, number2);
			});
			equipment_PurchaseButtons[k].onClick.AddListener(delegate
			{
				ClickedOnPurchaseSlot(number2);
			});
			UpdateSlot(isInventory: false, k);
		}
		Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.Gold] = (Action)Delegate.Combine(onCurrencyChange[Currencies.Gold], new Action(ManageAllPurchaseUnlockButtons));
		IDOfSelectedSlot = -1;
		isInventorySelected = false;
		ShowHideDetailsArea(show: false);
		if (playerData.instance.IsAutomaticallySellNormalAndRareItems)
		{
			AutoSellCheckmark.SetActive(value: true);
		}
		else
		{
			AutoSellCheckmark.SetActive(value: false);
		}
		if (playerData.instance.MonstersLevel >= 25)
		{
			AutoSellTextGO.SetActive(value: true);
		}
		else
		{
			AutoSellTextGO.SetActive(value: false);
		}
	}

	public void ChangeAutoSell()
	{
		playerData.instance.IsAutomaticallySellNormalAndRareItems = !playerData.instance.IsAutomaticallySellNormalAndRareItems;
		AutoSellCheckmark.SetActive(playerData.instance.IsAutomaticallySellNormalAndRareItems);
	}

	private void UpdateSlot(bool isInventory, int slotID)
	{
		SavedItemData savedItemData = (isInventory ? playerData.instance.Inventory[slotID] : playerData.instance.Equipment[slotID]);
		if (savedItemData == null)
		{
			GetHighlighters(isInventory)[slotID].SetActive(value: false);
			GetIcons(isInventory)[slotID].sprite = null;
			GetIconsGOs(isInventory)[slotID].SetActive(value: false);
			GetSlotImages(isInventory)[slotID].color = DefaultSlotColor;
		}
		else
		{
			GetIcons(isInventory)[slotID].sprite = ItemsIcons[savedItemData.rarity][savedItemData.iconID];
			GetIconsGOs(isInventory)[slotID].SetActive(value: true);
			FunctionsNeeded.ConstrainImageSize(GetIconsGOs(isInventory)[slotID].GetComponent<RectTransform>(), GetIcons(isInventory)[slotID], isInventory ? IconSize_Inventory.x : IconSize_Equipment.x, isInventory ? IconSize_Inventory.y : IconSize_Equipment.y);
			GetSlotImages(isInventory)[slotID].color = RarityColorsDict[savedItemData.rarity];
		}
		if (!isInventory)
		{
			if (!playerData.instance.EquipmentIsUnlocked[slotID])
			{
				equipment_Blackout[slotID].SetActive(value: true);
				equipment_PurchaseButtonGOs[slotID].SetActive(value: true);
				ManageAllPurchaseUnlockButtons();
			}
			else
			{
				equipment_Blackout[slotID].SetActive(value: false);
				equipment_PurchaseButtonGOs[slotID].SetActive(value: false);
			}
		}
	}

	public void ManageAllPurchaseUnlockButtons()
	{
		for (int i = 0; i < equipment_PurchaseButtonGOs.Count; i++)
		{
			if (!playerData.instance.EquipmentIsUnlocked[i])
			{
				equipment_PurchaseButtons[i].interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.Gold, DatabaseManager.EquipmentUnlockCost[i], IsSpendAlso: false);
			}
		}
	}

	public void ManageEquipUnEquipButton()
	{
		if (IDOfSelectedSlot != -1)
		{
			if (isInventorySelected)
			{
				int nextEmptySlot = GetNextEmptySlot(isInventory: false);
				DetailsArea_EquipUnEquipButton.interactable = nextEmptySlot >= 0;
			}
			else
			{
				int nextEmptySlot2 = GetNextEmptySlot(isInventory: true);
				DetailsArea_EquipUnEquipButton.interactable = nextEmptySlot2 >= 0;
			}
		}
	}

	public void CheckItemDrop()
	{
		if (!playerData.instance.UnlockedSystems[UnlockableSystems.Items])
		{
			return;
		}
		if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Items])
		{
			DropAnItem();
			AchievementsManager.instance.UnlockAchievement("DropRune");
			playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Items] = false;
		}
		else if (RunManager.instance.TimeSpentInRun >= 7f)
		{
			bool flag = FunctionsNeeded.IsHappened(playerData.instance.stats.ItemsChance.Total.RealValue + playerData.instance.UnluckyProtectionTotalForItems);
			int num = 0;
			int num2 = ((playerData.instance.MonstersLevel < 15) ? 1 : ((playerData.instance.MonstersLevel < 30) ? 2 : 3));
			float num3 = playerData.instance.stats.ItemsChance.Total.RealValue;
			while (flag && num < num2)
			{
				DropAnItem();
				num++;
				flag = FunctionsNeeded.IsHappened(num3);
				num3 *= 0.5f;
				playerData.instance.UnluckyProtectionTotalForItems = 0f;
			}
			if (num == 0 && playerData.instance.stats.ItemsChance.Total.RealValue < 18f)
			{
				playerData.instance.UnluckyProtectionTotalForItems += 0.5f;
			}
		}
		if (playerData.instance.MonstersLevel >= 25)
		{
			AutoSellTextGO.SetActive(value: true);
		}
		else
		{
			AutoSellTextGO.SetActive(value: false);
		}
	}

	public void DropAnItem()
	{
		if (!playerData.instance.UnlockedSystems[UnlockableSystems.Items])
		{
			return;
		}
		Rarity rarity = Rarity.Normal;
		if (playerData.instance.stats.Well_ItemsCannotBeNormal.Total.RealValue > 50)
		{
			rarity = Rarity.Rare;
		}
		if (playerData.instance.stats.Well_ItemsCannotBeRare.Total.RealValue > 50)
		{
			rarity = Rarity.Epic;
		}
		Rarity maximumRarity = Rarity.Normal;
		if (playerData.instance.isDroppedRareItem)
		{
			maximumRarity = Rarity.Rare;
		}
		if (playerData.instance.isDroppedEpicItem)
		{
			maximumRarity = Rarity.Epic;
		}
		if (playerData.instance.isDroppedLegendaryItem)
		{
			maximumRarity = Rarity.Legendary;
		}
		Rarity rarity2 = FunctionsNeeded.GetARandomRarity_Items((rarity == Rarity.Epic) ? 0f : playerData.instance.stats.ItemsRarity.Total.RealValue, rarity, maximumRarity);
		if (rarity == Rarity.Epic)
		{
			rarity2 = (FunctionsNeeded.IsHappened(90f) ? Rarity.Epic : Rarity.Legendary);
		}
		if (!playerData.instance.isDroppedRareItem && playerData.instance.MonstersLevel >= DatabaseManager.RareItemLevel)
		{
			rarity2 = Rarity.Rare;
			playerData.instance.isDroppedRareItem = true;
		}
		else if (!playerData.instance.isDroppedEpicItem && playerData.instance.MonstersLevel >= DatabaseManager.EpicItemLevel)
		{
			rarity2 = Rarity.Epic;
			playerData.instance.isDroppedEpicItem = true;
		}
		else if (!playerData.instance.isDroppedLegendaryItem && playerData.instance.MonstersLevel >= DatabaseManager.LegendaryItemLevel)
		{
			rarity2 = Rarity.Legendary;
			playerData.instance.isDroppedLegendaryItem = true;
		}
		playerData.instance.TotalItemsDropped_CurrentRun[rarity2]++;
		playerData.instance.TotalItemsDropped_FullGame[rarity2]++;
		bool flag = false;
		if (RunManager.instance.isDoubleLootRun)
		{
			flag = true;
			playerData.instance.TotalItemsDropped_CurrentRun[rarity2]++;
			playerData.instance.TotalItemsDropped_FullGame[rarity2]++;
		}
		if (playerData.instance.IsAutomaticallySellNormalAndRareItems && (rarity2 == Rarity.Normal || rarity2 == Rarity.Rare))
		{
			PlayerManager.instance.ChangeCurrency(Currencies.Gold, DatabaseManager.ItemSellPrice(rarity2, playerData.instance.MonstersLevel), isApplyGainMulti: false, isApplyAch: false);
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.Gold] += DatabaseManager.ItemSellPrice(rarity2, playerData.instance.MonstersLevel);
			if (flag)
			{
				PlayerManager.instance.ChangeCurrency(Currencies.Gold, DatabaseManager.ItemSellPrice(rarity2, playerData.instance.MonstersLevel), isApplyGainMulti: false, isApplyAch: false);
				playerData.instance.TotalCurrenciesGained_FullGame[Currencies.Gold] += DatabaseManager.ItemSellPrice(rarity2, playerData.instance.MonstersLevel);
			}
			return;
		}
		if (rarity2 == Rarity.Legendary)
		{
			AchievementsManager.instance.UnlockAchievement("DropRuneLegendary");
		}
		CreateANewItemForInventory(rarity2);
		if (flag)
		{
			CreateANewItemForInventory(rarity2);
		}
		MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Items, isShow: true);
	}

	public void CheckNotifications_EndRun()
	{
	}

	public void CreateANewItemForInventory(Rarity rarity)
	{
		int nextEmptySlot = GetNextEmptySlot(isInventory: true);
		if (nextEmptySlot >= 0)
		{
			int monstersLevel = playerData.instance.MonstersLevel;
			int iconID = UnityEngine.Random.Range(0, ItemsIcons[rarity].Count);
			SavedItemData value = new SavedItemData(nextEmptySlot, monstersLevel, DatabaseManager.AllItemStats.GetStatsAndRandomer(monstersLevel, rarity), rarity, iconID, playerData.instance.stats.Well_ItemsEffectMultiplier.Total.RealValue);
			playerData.instance.Inventory[nextEmptySlot] = value;
			UpdateSlot(isInventory: true, nextEmptySlot);
			ClickedOnSlot(isInventory: true, nextEmptySlot);
		}
	}

	public void ClickedOnEquipUnEquipButton()
	{
		if (IDOfSelectedSlot == -1)
		{
			return;
		}
		if (isInventorySelected)
		{
			int nextEmptySlot = GetNextEmptySlot(isInventory: false);
			if (nextEmptySlot >= 0)
			{
				playerData.instance.Equipment[nextEmptySlot] = playerData.instance.Inventory[IDOfSelectedSlot];
				playerData.instance.Inventory[IDOfSelectedSlot] = null;
				playerData.instance.TotalStatsInItemsEquipped += playerData.instance.Equipment[nextEmptySlot].statsAndRandomer.Count;
				ApplyRemoveItemStats(playerData.instance.Equipment[nextEmptySlot], isApply: true);
				UpdateSlot(isInventory: false, nextEmptySlot);
				UpdateSlot(isInventory: true, IDOfSelectedSlot);
				ClickedOnSlot(isInventory: false, nextEmptySlot);
			}
		}
		else
		{
			int nextEmptySlot2 = GetNextEmptySlot(isInventory: true);
			if (nextEmptySlot2 >= 0)
			{
				playerData.instance.Inventory[nextEmptySlot2] = playerData.instance.Equipment[IDOfSelectedSlot];
				playerData.instance.Equipment[IDOfSelectedSlot] = null;
				playerData.instance.TotalStatsInItemsEquipped -= playerData.instance.Inventory[nextEmptySlot2].statsAndRandomer.Count;
				ApplyRemoveItemStats(playerData.instance.Inventory[nextEmptySlot2], isApply: false);
				UpdateSlot(isInventory: false, IDOfSelectedSlot);
				UpdateSlot(isInventory: true, nextEmptySlot2);
				ClickedOnSlot(isInventory: true, nextEmptySlot2);
			}
		}
	}

	public void ClickedOnSlot(bool isInventory, int slotID)
	{
		if ((!isInventory || playerData.instance.Inventory[slotID] != null) && (isInventory || playerData.instance.Equipment[slotID] != null))
		{
			if (IDOfSelectedSlot >= 0)
			{
				GetHighlighters(isInventorySelected)[IDOfSelectedSlot].SetActive(value: false);
			}
			GetHighlighters(isInventory)[slotID].SetActive(value: true);
			IDOfSelectedSlot = slotID;
			isInventorySelected = isInventory;
			UpdateDetailsArea();
		}
	}

	private void UpdateDetailsArea()
	{
		SavedItemData savedItemData = (isInventorySelected ? playerData.instance.Inventory[IDOfSelectedSlot] : playerData.instance.Equipment[IDOfSelectedSlot]);
		if (savedItemData == null)
		{
			return;
		}
		ShowHideDetailsArea(show: true);
		DetailsArea_Icon.sprite = ItemsIcons[savedItemData.rarity][savedItemData.iconID];
		DetailsArea_SlotImage.color = RarityColorsDict[savedItemData.rarity];
		string text = "";
		int num = 0;
		string text2 = "";
		foreach (KeyValuePair<string, double> item in savedItemData.statsAndRandomer)
		{
			StatInfo statInfo = DatabaseManager.StatsDict[item.Key];
			text += GetSkillNameFromStatName(item.Key);
			text += statInfo.GetValueDescText_SingleOrMultipleValues(new List<double> { savedItemData.GetStatValue(item.Key) }, isColoredTag: false);
			text2 = text2 + "(" + DatabaseManager.AllItemStats.StatsDict[item.Key].Value.x + " - " + DatabaseManager.AllItemStats.StatsDict[item.Key].Value.y + ")";
			num++;
			if (num < savedItemData.statsAndRandomer.Count)
			{
				text += "\n";
				text2 += "\n";
			}
		}
		DetailsArea_ModText.text = text;
		DetailsArea_ModText.ForceMeshUpdate();
		Vector2 preferredValues = DetailsArea_ModText.GetPreferredValues();
		DetailsArea_ModText.rectTransform.sizeDelta = new Vector2(Mathf.Clamp(preferredValues.x, 0f, maxStatsTextWidth), DetailsArea_ModText.rectTransform.sizeDelta.y);
		DetailsArea_ModValueRangeText.text = text2;
		DetailsArea_LevelText.text = LocalizerManager.GetTranslatedValue("Lv_Text") + " " + savedItemData.itemLevel;
		DetailsArea_RarityText.text = LocalizerManager.GetTranslatedValue(savedItemData.rarity.ToString() + "_Text");
		DetailsArea_RarityText.color = RarityColorsDict_Text[savedItemData.rarity];
		FunctionsNeeded.ConstrainImageSize(DetailsArea_Icon.GetComponent<RectTransform>(), DetailsArea_Icon, IconSize_DetailsArea.x, IconSize_DetailsArea.y);
		ManageEquipUnEquipButton();
		DetailsArea_EquipUnEquipButtonText.text = LocalizerManager.GetTranslatedValue(isInventorySelected ? "Equip_Text" : "Unequip_Text");
		DetailsArea_SellCostText.text = LocalizerManager.GetTranslatedValue("Sell_Text") + " <sprite name=Gold>" + DatabaseManager.ItemSellPrice(savedItemData.rarity, savedItemData.itemLevel).ToReadable();
	}

	public void ClickedOnSellSelectedItem()
	{
		GetHighlighters(isInventorySelected)[IDOfSelectedSlot].SetActive(value: false);
		SavedItemData savedItemData = (isInventorySelected ? playerData.instance.Inventory[IDOfSelectedSlot] : playerData.instance.Equipment[IDOfSelectedSlot]);
		if (savedItemData != null)
		{
			if (isInventorySelected)
			{
				playerData.instance.Inventory[IDOfSelectedSlot] = null;
			}
			else
			{
				ApplyRemoveItemStats(savedItemData, isApply: false);
				playerData.instance.TotalStatsInItemsEquipped -= playerData.instance.Equipment[IDOfSelectedSlot].statsAndRandomer.Count;
				playerData.instance.Equipment[IDOfSelectedSlot] = null;
			}
			ShowHideDetailsArea(show: false);
			PlayerManager.instance.ChangeCurrency(Currencies.Gold, DatabaseManager.ItemSellPrice(savedItemData.rarity, savedItemData.itemLevel), isApplyGainMulti: false, isApplyAch: false);
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.Gold] += DatabaseManager.ItemSellPrice(savedItemData.rarity, savedItemData.itemLevel);
			UpdateSlot(isInventorySelected, IDOfSelectedSlot);
			IDOfSelectedSlot = -1;
		}
	}

	private void ShowHideDetailsArea(bool show)
	{
		DetailsArea_EquipUnEquipButtonGO.SetActive(show);
		DetailsArea_SellButtonGO.SetActive(show);
		DetailsArea_ModTextGO.SetActive(show);
		DetailsArea_LevelTextGO.SetActive(show);
		DetailsArea_RarityTextGO.SetActive(show);
		DetailsArea_IconGO.SetActive(show);
		DetailsArea_SlotImageGO.SetActive(show);
		DetailsArea_ModValueRangeTextGO.SetActive(show);
	}

	public void ClickedOnPurchaseSlot(int slotID)
	{
		if (!playerData.instance.EquipmentIsUnlocked[slotID] && PlayerManager.instance.IsCanSpendCurrency(Currencies.Gold, DatabaseManager.EquipmentUnlockCost[slotID], IsSpendAlso: true))
		{
			FXManager.instance.PlayUIClickSound();
			playerData.instance.EquipmentIsUnlocked[slotID] = true;
			UpdateSlot(isInventory: false, slotID);
			ManageAllPurchaseUnlockButtons();
			ManageEquipUnEquipButton();
		}
	}

	private int GetNextEmptySlot(bool isInventory)
	{
		int result = -1;
		for (int i = 0; i < (isInventory ? playerData.instance.Inventory.Count : playerData.instance.Equipment.Count); i++)
		{
			if (isInventory)
			{
				if (playerData.instance.Inventory[i] == null)
				{
					return i;
				}
			}
			else if (playerData.instance.EquipmentIsUnlocked[i] && playerData.instance.Equipment[i] == null)
			{
				return i;
			}
		}
		return result;
	}

	private void ApplyRemoveItemStats(SavedItemData sid, bool isApply)
	{
		foreach (KeyValuePair<string, double> item in sid.statsAndRandomer)
		{
			StatInfo statInfo = DatabaseManager.StatsDict[item.Key];
			double statValue = sid.GetStatValue(item.Key);
			playerData.instance.stats.ChangeAStat(statInfo.VariableName, statInfo.StatsProp, statValue, isApply);
		}
	}

	private string GetSkillNameFromStatName(string statName)
	{
		if (statName.Contains("LC_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("LightningChain_Name") + "</b>: ";
		}
		if (statName.Contains("Knight_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("KnightSlash_Name") + "</b>: ";
		}
		if (statName.Contains("RoA_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("RainOfArrows_Name") + "</b>: ";
		}
		if (statName.Contains("Vampire_"))
		{
			return "<b>" + LocalizerManager.GetTranslatedValue("VampireExplosion_Name") + "</b>: ";
		}
		return "";
	}

	private List<GameObject> GetHighlighters(bool isInventory)
	{
		if (!isInventory)
		{
			return equipment_Highlighters;
		}
		return inventory_Highlighters;
	}

	private List<Image> GetIcons(bool isInventory)
	{
		if (!isInventory)
		{
			return equipment_Icons;
		}
		return inventory_Icons;
	}

	private List<GameObject> GetIconsGOs(bool isInventory)
	{
		if (!isInventory)
		{
			return equipment_IconsGOs;
		}
		return inventory_IconsGOs;
	}

	private List<Image> GetSlotImages(bool isInventory)
	{
		if (!isInventory)
		{
			return equipment_SlotImages;
		}
		return inventory_SlotImages;
	}
}
public class SavedItemData
{
	public int inventorySlotID;

	public int itemLevel;

	public StringDouble statsAndRandomer;

	public Rarity rarity;

	public int iconID;

	public float ValuesMultiplier;

	public SavedItemData(int inventorySlotID, int itemLevel, StringDouble statsAndRandomer, Rarity rarity, int iconID, float ValuesMultiplier)
	{
		this.inventorySlotID = inventorySlotID;
		this.itemLevel = itemLevel;
		this.statsAndRandomer = statsAndRandomer;
		this.rarity = rarity;
		this.iconID = iconID;
		this.ValuesMultiplier = ValuesMultiplier;
	}

	public double GetStatValue(string statName)
	{
		int roundToNearest = DatabaseManager.AllItemStats.StatsDict[statName].Stat.RoundToNearest;
		Vector2 value = DatabaseManager.AllItemStats.StatsDict[statName].Value;
		float num = ValuesMultiplier * Mathf.Lerp(value.x, value.y, (float)statsAndRandomer[statName]);
		if (num > 1f && FunctionsNeeded.ApproximatelyEqualEpsilon(value.x, 1.0) && (FunctionsNeeded.ApproximatelyEqualEpsilon(value.y, 2.0) || FunctionsNeeded.ApproximatelyEqualEpsilon(value.y, 1.0)) && (double)(Mathf.Ceil(num) - num) > 0.25)
		{
			return Math.Floor(num);
		}
		return Math.Round(num, roundToNearest);
	}
}
[CreateAssetMenu]
public class ItemStatsInfo : SerializedScriptableObject
{
	public List<ItemStat> Stats;

	[HideInInspector]
	public Dictionary<string, ItemStat> StatsDict;

	private Dictionary<Rarity, int> NumberOfStatsPerRarity = new Dictionary<Rarity, int>
	{
		{
			Rarity.Normal,
			1
		},
		{
			Rarity.Rare,
			2
		},
		{
			Rarity.Epic,
			3
		},
		{
			Rarity.Legendary,
			4
		}
	};

	[HideInInspector]
	public List<string> AllMetStats = new List<string>();

	public void AwakeMe()
	{
		StatsDict = new Dictionary<string, ItemStat>();
		foreach (ItemStat stat in Stats)
		{
			StatsDict.Add(stat.Stat.functionName, stat);
		}
		AllMetStats = new List<string>();
	}

	public void CheckMetStats()
	{
		foreach (ItemStat stat in Stats)
		{
			if (!AllMetStats.Contains(stat.Stat.functionName) && stat.Condition.IsMet())
			{
				AllMetStats.Add(stat.Stat.functionName);
			}
		}
	}

	public StringDouble GetStatsAndRandomer(int itemLevel, Rarity rarity)
	{
		CheckMetStats();
		if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Items])
		{
			return new StringDouble { { "GoldGainedAdditive", 0.30000001192092896 } };
		}
		List<string> list = new List<string>(AllMetStats);
		StringDouble stringDouble = new StringDouble();
		for (int i = 0; i < NumberOfStatsPerRarity[rarity]; i++)
		{
			string oneRandom = list.GetOneRandom();
			list.Remove(oneRandom);
			stringDouble.Add(oneRandom, DatabaseManager.GetBiasedRandom(itemLevel));
		}
		return stringDouble;
	}
}
[Serializable]
public class ItemStat
{
	public StatInfo Stat;

	public Vector2 Value;

	[SerializeReference]
	public AzrarCondition Condition = new Cond_AlwaysMet();

	public ItemStat()
	{
		Condition = new Cond_AlwaysMet();
	}

	public ItemStat(StatInfo Stat, Vector2 Value, AzrarCondition Condition = null)
	{
		this.Stat = Stat;
		this.Value = Value;
		this.Condition = Condition ?? new Cond_AlwaysMet();
	}
}
[CreateAssetMenu]
public class LevelInfo : SerializedScriptableObject
{
	public int Level;

	public double UnlockCost;

	public Dictionary<Currencies, double> CurrenciesDropped;
}
public class AchievementsManager : MonoBehaviour
{
	public static AchievementsManager instance;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (playerData.instance.TotalCurrenciesGained_FullGame[Currencies.ClearCurrency] >= 39.0)
		{
			instance.UnlockAchievement("TreeFull_Unlocked");
		}
	}

	public void UnlockAchievement(string achName)
	{
		SteamUserStats.GetAchievement(achName, out var pbAchieved);
		if (!pbAchieved)
		{
			SteamUserStats.SetAchievement(achName);
			SteamUserStats.StoreStats();
		}
	}
}
public class FloatingNumbersManager : MonoBehaviour
{
	public static FloatingNumbersManager instance;

	public Vector2 Randomization;

	private Dictionary<int, TextSelfFloating> AllTextSelfFloating = new Dictionary<int, TextSelfFloating>();

	public int HowManyActive_DamageNumbers;

	[HideInInspector]
	public int HowManyActive_GoldNumbers;

	public Transform UIFloaterParent;

	public Transform PlayerTakeDamageFloaterParent;

	public Color PositiveColor;

	public Color NegativeColor;

	private List<string> FloatingNumbersForHeartPositions = new List<string> { "PlayerTakeDamageFloater", "PlayerRegenHealthFloater" };

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		HowManyActive_DamageNumbers = 0;
		HowManyActive_GoldNumbers = 0;
		AllTextSelfFloating = new Dictionary<int, TextSelfFloating>();
	}

	public void GenerateUIFloatingNumber(string ThingToShow, Vector3 Pos, bool isPositive)
	{
		GameObject gameObject = ObjectPooler.instance.GiveMeObject("UIFloater", UIFloaterParent, Pos);
		gameObject.transform.position = (Vector2)Pos + UnityEngine.Random.insideUnitCircle * 10f;
		int hashCode = gameObject.GetHashCode();
		if (!AllTextSelfFloating.ContainsKey(hashCode))
		{
			AllTextSelfFloating.Add(hashCode, gameObject.GetComponent<TextSelfFloating>());
		}
		if (isPositive)
		{
			AllTextSelfFloating[hashCode].OriginalColor = PositiveColor;
		}
		else
		{
			AllTextSelfFloating[hashCode].OriginalColor = NegativeColor;
		}
		bool isInit = AllTextSelfFloating[hashCode].isInit;
		AllTextSelfFloating[hashCode].TakeInfo(ThingToShow);
		if (!isInit)
		{
			if (isPositive)
			{
				AllTextSelfFloating[hashCode].MyTextMeshPro.color = PositiveColor;
			}
			else
			{
				AllTextSelfFloating[hashCode].MyTextMeshPro.color = NegativeColor;
			}
		}
	}

	public void GenerateFloatingNumber(string ThingToShow, Vector3 Pos, string WhichFloater = "MonsterTakeDamageFloater")
	{
		if (!playerData.instance.isDamageFloatingShown)
		{
			return;
		}
		float num = 0f;
		if (HowManyActive_DamageNumbers > 300)
		{
			num = 80f;
		}
		else if (HowManyActive_DamageNumbers > 250)
		{
			num = 70f;
		}
		else if (HowManyActive_DamageNumbers > 200)
		{
			num = 50f;
		}
		else if (HowManyActive_DamageNumbers > 150)
		{
			num = 20f;
		}
		if (!(num > 10f) || !FunctionsNeeded.IsHappened(num))
		{
			HowManyActive_DamageNumbers++;
			GameObject gameObject = ObjectPooler.instance.GiveMeObject(WhichFloater, FloatingNumbersForHeartPositions.Contains(WhichFloater) ? PlayerTakeDamageFloaterParent : base.transform, Pos);
			Vector3 vector = new Vector3(UnityEngine.Random.Range(0f - Randomization.x, Randomization.x), UnityEngine.Random.Range(0f - Randomization.y, Randomization.y), -3f);
			gameObject.transform.position = Pos + vector;
			int hashCode = gameObject.GetHashCode();
			if (!AllTextSelfFloating.ContainsKey(hashCode))
			{
				AllTextSelfFloating.Add(hashCode, gameObject.GetComponent<TextSelfFloating>());
			}
			AllTextSelfFloating[hashCode].TakeInfo(ThingToShow);
		}
	}
}
public enum TypeOfHits
{
	NormalToEnemy,
	Critial,
	Dodge,
	Missed,
	Dead
}
public class FXManager : MonoBehaviour
{
	public static FXManager instance;

	private List<string> ListOfUselessEfects = new List<string>();

	public AudioSource LoopGoldSound;

	[Header("Sound Throttling Settings")]
	[Tooltip("Minimum time (in seconds) between playing the same sound. Lower = more frequent sounds. Recommended: 0.03-0.1")]
	public float minTimeBetweenSameSounds = 0.03f;

	[Tooltip("Maximum number of the same sound that can play simultaneously. Lower = fewer overlapping sounds. Recommended: 2-5")]
	public int maxConcurrentSameSounds = 10;

	private List<string> soundsToBypassThrottling = new List<string>();

	private Dictionary<string, float> lastSoundPlayTime = new Dictionary<string, float>();

	private Dictionary<string, int> activeSoundCount = new Dictionary<string, int>();

	private Dictionary<string, Queue<float>> soundTimestamps = new Dictionary<string, Queue<float>>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		ListOfUselessEfects.Add("ClickHitPrefab");
		ListOfUselessEfects.Add("LightningStrike");
		ListOfUselessEfects.Add("SuperNova");
		ListOfUselessEfects.Add("FiringDustGFXPrefab");
		soundsToBypassThrottling.Add("EnemySpawnSound");
	}

	public GameObject SpawnGFX(string prefabName, Vector3 Pos, float duration, Vector3 Scale, float Angle = 0f, bool ForceShow = false)
	{
		if (ListOfUselessEfects.Contains(prefabName))
		{
			return null;
		}
		if (playerData.instance.EffectsAmount < 99f && !ForceShow && FunctionsNeeded.IsHappened(100f - playerData.instance.EffectsAmount))
		{
			return null;
		}
		bool IsInstatiated;
		GameObject gameObject = ObjectPooler.instance.GiveMeObject(prefabName, base.transform, Pos, out IsInstatiated);
		if (Scale.x > 0f)
		{
			gameObject.transform.localScale = Scale;
		}
		gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Angle);
		if (IsInstatiated)
		{
			gameObject.GetComponent<PSSelfer>().TakeInfo(prefabName, duration);
		}
		return gameObject;
	}

	public void PlaySound(string prefabName, bool ForcePlay = false)
	{
		if ((!(playerData.instance.SoundVolume_Effects <= 5f) || !(playerData.instance.SoundVolume_UI <= 5f)) && ObjectPooler.instance.ObjectsToBePooled.ContainsKey(prefabName) && (!(playerData.instance.EffectsAmount < 99f) || ForcePlay || !FunctionsNeeded.IsHappened(100f - playerData.instance.EffectsAmount)) && !ShouldThrottleSound(prefabName))
		{
			bool IsInstatiated;
			GameObject obj = ObjectPooler.instance.GiveMeObject(prefabName, base.transform, Vector3.zero, out IsInstatiated);
			SoundSelfer component = obj.GetComponent<SoundSelfer>();
			if (IsInstatiated)
			{
				component.MyNameOnObjectPooler = prefabName;
			}
			float clipDuration = 5f;
			AudioSource component2 = obj.GetComponent<AudioSource>();
			if (component2 != null && component2.clip != null)
			{
				clipDuration = component2.clip.length + 0.1f;
			}
			TrackSoundPlayback(prefabName, clipDuration);
		}
	}

	private bool ShouldThrottleSound(string soundName)
	{
		if (soundsToBypassThrottling.Contains(soundName))
		{
			return false;
		}
		float time = Time.time;
		if (lastSoundPlayTime.ContainsKey(soundName) && time - lastSoundPlayTime[soundName] < minTimeBetweenSameSounds)
		{
			return true;
		}
		if (activeSoundCount.ContainsKey(soundName) && activeSoundCount[soundName] >= maxConcurrentSameSounds)
		{
			return true;
		}
		return false;
	}

	private void TrackSoundPlayback(string soundName, float clipDuration)
	{
		float time = Time.time;
		lastSoundPlayTime[soundName] = time;
		if (!activeSoundCount.ContainsKey(soundName))
		{
			activeSoundCount[soundName] = 0;
		}
		activeSoundCount[soundName]++;
		if (!soundTimestamps.ContainsKey(soundName))
		{
			soundTimestamps[soundName] = new Queue<float>();
		}
		soundTimestamps[soundName].Enqueue(time);
		StartCoroutine(CleanupSoundTracking(soundName, clipDuration));
	}

	private IEnumerator CleanupSoundTracking(string soundName, float clipDuration)
	{
		yield return new WaitForSeconds(clipDuration);
		if (activeSoundCount.ContainsKey(soundName))
		{
			activeSoundCount[soundName]--;
			if (activeSoundCount[soundName] <= 0)
			{
				activeSoundCount[soundName] = 0;
			}
		}
		if (soundTimestamps.ContainsKey(soundName))
		{
			while (soundTimestamps[soundName].Count > 0 && Time.time - soundTimestamps[soundName].Peek() > clipDuration)
			{
				soundTimestamps[soundName].Dequeue();
			}
		}
	}

	public void OnSoundFinished(string soundName)
	{
		if (activeSoundCount.ContainsKey(soundName))
		{
			activeSoundCount[soundName]--;
			if (activeSoundCount[soundName] <= 0)
			{
				activeSoundCount[soundName] = 0;
			}
		}
	}

	public void PlayGeneralSound(GeneralSounds sound)
	{
		PlaySound(sound.ToString(), ForcePlay: true);
	}

	public void PlayUIClickSound()
	{
		PlayGeneralSound(GeneralSounds.button_click);
	}

	public void PlayStopLoopGoldSound(bool isPlay)
	{
		if (isPlay)
		{
			if (LoopGoldSound.clip != null)
			{
				float time = UnityEngine.Random.Range(0f, LoopGoldSound.clip.length);
				LoopGoldSound.time = time;
			}
			LoopGoldSound.Play();
		}
		else
		{
			LoopGoldSound.Stop();
		}
	}
}
public enum GeneralSounds
{
	button_click,
	button_hover,
	card_activation,
	card_click,
	card_discard,
	card_draw,
	card_hover,
	generic_ui_hover,
	gold_gain_one,
	purchase,
	relic_activation,
	send_to_play,
	card_declick,
	gain_add,
	gain_mult,
	calculation,
	menu_whoosh,
	calculation_show,
	start_attack,
	ascend_appear,
	stat_winlose_tick,
	reward_plate_appear,
	lose,
	space_enter_tick
}
[CreateAssetMenu]
public class GroundClickableInfo : SerializedScriptableObject
{
	[HideInInspector]
	public string functionName;

	public Sprite icon;

	public float size = 1f;

	public Dictionary<string, double> values = new Dictionary<string, double>();

	public float chance = 100f;
}
public class GroundClickableManager : MonoBehaviour
{
	public static GroundClickableManager instance;

	private bool isCurrentClickableTaken;

	private GameObject CurrentClickableGO;

	private GroundClickableInfo CurrentClickableInfo;

	private float GroundClickableTimer;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		isCurrentClickableTaken = true;
	}

	public void StartRun()
	{
		GroundClickableTimer = DatabaseManager.BaseGroundClickableCheckEverySecond;
		if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Bounties])
		{
			GroundClickableTimer *= 0.5f;
		}
	}

	public void EndRun()
	{
		RemoveAllGroundClickables();
	}

	private void SpawnGroundClickable()
	{
		if (!isCurrentClickableTaken)
		{
			ObjectPooler.instance.ReturnObjectToPool(CurrentClickableGO, "GroundClickablePrefab");
		}
		CurrentClickableInfo = GetARandomGroundClickable();
		isCurrentClickableTaken = false;
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < 10; i++)
		{
			list.Add(EnemiesManager.instance.SampleRandomPointInTrapezoid(isAvoidEnemies: true));
		}
		List<Vector2> list2 = EnemiesManager.instance.AliveEnemies_Transform.Values.Select((Transform x) => new Vector2(x.position.x, x.position.y)).ToList();
		Vector2 vector = list[0];
		if (CurrentClickableInfo.functionName == "Damage_AoE")
		{
			do
			{
				Vector2 vector2 = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(90, 130);
				vector = list2.GetOneRandom() + vector2;
			}
			while (!EnemiesManager.instance.IsPointInTrapezoid(vector));
		}
		else
		{
			float num = float.MinValue;
			foreach (Vector2 item in list)
			{
				float num2 = float.MaxValue;
				foreach (Vector2 item2 in list2)
				{
					float num3 = Vector2.Distance(item, item2);
					if (num3 < num2)
					{
						num2 = num3;
					}
				}
				if (num2 > num)
				{
					num = num2;
					vector = item;
				}
			}
		}
		CurrentClickableGO = ObjectPooler.instance.GiveMeObject("GroundClickablePrefab", base.transform, vector);
		CurrentClickableGO.GetComponent<GroundClickableSelfer>().TakeInfo(CurrentClickableInfo);
		AutomatorBot.instance.BountyFound(isTaken: false, CurrentClickableGO.transform.position);
	}

	public void CheckBountyUnlocks()
	{
		if (playerData.instance.stats.Bounty_SpawnShiny_CanBeFoundInRun.Total.RealValue > 0)
		{
			playerData.instance.GroundClickableIsUnlocked["SpawnShiny"] = true;
		}
		if (playerData.instance.stats.Bounty_DropItem_CanBeFoundInRun.Total.RealValue > 0)
		{
			playerData.instance.GroundClickableIsUnlocked["DropItem"] = true;
		}
		if (playerData.instance.stats.Bounty_CallSkills_CanBeFoundInRun.Total.RealValue > 0)
		{
			playerData.instance.GroundClickableIsUnlocked["CallSkills"] = true;
		}
	}

	private GroundClickableInfo GetARandomGroundClickable()
	{
		List<GroundClickableInfo> list = DatabaseManager.GroundClickableList.Where((GroundClickableInfo a) => playerData.instance.GroundClickableIsUnlocked[a.functionName]).ToList();
		float maxInclusive = list.Sum((GroundClickableInfo a) => a.chance);
		float num = UnityEngine.Random.Range(0f, maxInclusive);
		float num2 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			num2 += list[i].chance;
			if (num <= num2)
			{
				return list[i];
			}
		}
		return list[0];
	}

	public void ClickedOnClickable(Vector2 Pos)
	{
		DoClickableFunction(CurrentClickableInfo.functionName, Pos);
		FXManager.instance.SpawnGFX("ClickOnGroundClickablePrefab", CurrentClickableGO.transform.position, 2f, Vector3.one * -1f, 0f, ForceShow: true);
		FXManager.instance.PlaySound("ClickableGainSound", ForcePlay: true);
		isCurrentClickableTaken = true;
		ObjectPooler.instance.ReturnObjectToPool(CurrentClickableGO, "GroundClickablePrefab");
	}

	public void DoClickableFunction(string ClickableFunction, Vector2 Pos)
	{
		playerData.instance.TotalBountiesFound_CurrentRun++;
		GroundClickableInfo groundClickableInfo = DatabaseManager.GroundClickableDict[ClickableFunction];
		switch (ClickableFunction)
		{
		case "Gold_Gain":
		{
			double num3 = DatabaseManager.EnemyGold(playerData.instance.MonstersLevel) * groundClickableInfo.values["GoldMultiplier"] * (double)playerData.instance.stats.BountiesEffect.Total.RealValue;
			num3 *= (double)playerData.instance.stats.GoldGained.Total.RealValue;
			num3 = Math.Ceiling(num3);
			string text = num3.ToReadable();
			if (RunManager.instance.isDoubleGoldRun && RunManager.instance.isDoubleLootRun)
			{
				num3 *= 4.0;
			}
			else if (RunManager.instance.isDoubleGoldRun || RunManager.instance.isDoubleLootRun)
			{
				num3 *= 2.0;
			}
			FloatingNumbersManager.instance.GenerateFloatingNumber("<sprite name=Gold>" + text, Pos, "GoldFloater");
			playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.Gold] += num3;
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.Gold] += num3;
			PlayerManager.instance.ChangeCurrency(Currencies.Gold, num3, isApplyGainMulti: false);
			break;
		}
		case "Damage_Projectile":
		{
			int howMany = Mathf.FloorToInt((float)groundClickableInfo.values["NumberOfEnemies"] * playerData.instance.stats.BountiesEffect.Total.RealValue);
			double multiplier = groundClickableInfo.values["DamageMultiplier"] * (double)playerData.instance.stats.BountiesEffect.Total.RealValue;
			List<EnemySelfer> nearestEnemies = EnemiesManager.instance.GetNearestEnemies(Pos, howMany);
			DamageData dd = playerData.instance.stats.DamageCalculation("GroundHitMultipleEnemies", "damage1", multiplier);
			if (nearestEnemies != null)
			{
				for (int j = 0; j < nearestEnemies.Count; j++)
				{
					float f = FunctionsNeeded.CalculateAngle((Vector2)nearestEnemies[j].transform.position - Pos, IsRadian: true);
					Vector2 direction = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
					ProjectilesManager.instance.FireSingleHitMultiEnemiesGroundClickable(Pos, direction, DatabaseManager.ProjectileDict["HitMultiEnemiesGroundClickable_Projectile"], dd);
				}
			}
			break;
		}
		case "Damage_AoE":
		{
			float radius = (float)groundClickableInfo.values["RadiusOfEffect"];
			float duration = (float)groundClickableInfo.values["DurationOfEffect"] * playerData.instance.stats.BountiesEffect.Total.RealValue;
			float tickTime = (float)groundClickableInfo.values["TickTimeOfEffect"];
			double multiplier2 = groundClickableInfo.values["DamageMultiplier"] * (double)playerData.instance.stats.BountiesEffect.Total.RealValue;
			DamageData damage = playerData.instance.stats.DamageCalculation("Bounty_Damage_AoE", "damage1", multiplier2);
			GroundEffectsManager.instance.SpawnGround("ClickableGroundEffectPrefab", Pos, radius, duration, tickTime, damage);
			break;
		}
		case "CallSkills":
		{
			int howmanyTimes = Mathf.FloorToInt((float)groundClickableInfo.values["HowManyTimes"] * playerData.instance.stats.BountiesEffect.Total.RealValue);
			SkillBarsManager.instance.CallAllSkills(howmanyTimes, Pos);
			break;
		}
		case "SpawnShiny":
		{
			int num2 = Mathf.FloorToInt((float)groundClickableInfo.values["HowManyTimes"] * playerData.instance.stats.BountiesEffect.Total.RealValue);
			for (int k = 0; k < num2; k++)
			{
				EnemiesManager.instance.CreateAnEnemy(isStart: false, Pos + new Vector2(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50)), EnemiesManager.ForceMonsterSpawnType.Shiny);
			}
			break;
		}
		case "DropItem":
		{
			int num = (int)((float)groundClickableInfo.values["HowManyTimes"] * playerData.instance.stats.BountiesEffect.Total.RealValue);
			for (int i = 0; i < num; i++)
			{
				InventoryManager.instance.DropAnItem();
			}
			break;
		}
		}
		AutomatorBot.instance.BountyFound(isTaken: true, Pos);
	}

	public void RemoveAllGroundClickables()
	{
		isCurrentClickableTaken = true;
		if (CurrentClickableGO != null)
		{
			ObjectPooler.instance.ReturnObjectToPool(CurrentClickableGO, "GroundClickablePrefab");
		}
	}

	public void DoReset()
	{
		RemoveAllGroundClickables();
	}

	private void Update()
	{
		if (!RunManager.instance.isRunStarted || !playerData.instance.UnlockedSystems[UnlockableSystems.Bounties])
		{
			return;
		}
		GroundClickableTimer -= Time.deltaTime;
		if (GroundClickableTimer <= 0f)
		{
			GroundClickableTimer = DatabaseManager.BaseGroundClickableCheckEverySecond;
			if (playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Bounties] || FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceToSpawnBounty.Total.RealValue))
			{
				playerData.instance.IsJustUnlockedSystem[UnlockableSystems.Bounties] = false;
				SpawnGroundClickable();
			}
		}
	}
}
public class GroundClickableSelfer : MonoBehaviour
{
	private bool isInit;

	private SpriteRenderer mySR;

	private Transform myGFXTrans;

	private Tween tw;

	private Coroutine AutoCollectBountiesCoroutine;

	public void TakeInfo(GroundClickableInfo myInfo)
	{
		if (!isInit)
		{
			isInit = true;
			mySR = base.transform.Find("GFX").GetComponent<SpriteRenderer>();
			myGFXTrans = base.transform.Find("GFX");
		}
		mySR.sprite = myInfo.icon;
		mySR.sortingOrder = Mathf.RoundToInt((0f - base.transform.position.y) * 10f);
		myGFXTrans.localScale = myInfo.size * Vector3.one;
		mySR.transform.localPosition = new Vector3(0f, 13f, 0f);
		if (tw != null)
		{
			tw.Kill();
		}
		tw = mySR.transform.DOLocalMoveY(35f, 2f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		if (AutoCollectBountiesCoroutine != null)
		{
			StopCoroutine(AutoCollectBountiesCoroutine);
			AutoCollectBountiesCoroutine = null;
		}
		if (playerData.instance.stats.Well_BountiesAutoCollect.Total.RealValue > 50)
		{
			AutoCollectBountiesCoroutine = StartCoroutine(AutoCollectBounties());
		}
	}

	private IEnumerator AutoCollectBounties()
	{
		yield return new WaitForSeconds(0.5f);
		ClickedOnClickable();
	}

	public void ClickedOnClickable()
	{
		GroundClickableManager.instance.ClickedOnClickable(base.transform.position);
	}

	private void OnMouseEnter()
	{
		ClickedOnClickable();
	}
}
public class GroundEffectSelfer : MonoBehaviour
{
	private bool isStart;

	private float radius;

	private float duration;

	private float tickTime;

	private DamageData damage;

	private GameObject GFXGO;

	public float ScaleMultiplier;

	public string prefabName;

	private bool isInit;

	private List<ParticleSystem> AllChildPS = new List<ParticleSystem>();

	private float currentTime;

	private float currentDuration;

	public void TakeInfo(float radius, float duration, float tickTime, DamageData damage)
	{
		CancelInvoke();
		if (!isInit)
		{
			isInit = true;
			GFXGO = base.transform.Find("GFX").gameObject;
			AllChildPS.Add(GFXGO.GetComponent<ParticleSystem>());
			foreach (Transform item in GFXGO.transform)
			{
				AllChildPS.Add(item.GetComponent<ParticleSystem>());
			}
		}
		for (int i = 0; i < AllChildPS.Count; i++)
		{
			AllChildPS[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			ParticleSystem.MainModule main = AllChildPS[i].main;
			main.duration = duration;
			AllChildPS[i].Play();
		}
		GFXGO.SetActive(value: true);
		isStart = true;
		this.radius = radius;
		this.duration = duration;
		this.tickTime = tickTime;
		this.damage = damage;
		GFXGO.transform.localScale = Vector3.one * ScaleMultiplier * radius * DatabaseManager.OneGameUnitToUnityUnit;
		if (prefabName == "ClickableGroundEffectPrefab")
		{
			ApplyTick();
		}
		currentTime = 0f;
		currentDuration = 0f;
	}

	public void ApplyTick()
	{
		List<EnemySelfer> enemiesInCircle = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(base.transform.position, radius * DatabaseManager.OneGameUnitToUnityUnit);
		for (int i = 0; i < enemiesInCircle.Count; i++)
		{
			enemiesInCircle[i].TakeDamage(damage);
		}
	}

	public void EndEffect()
	{
		isStart = false;
		Invoke("DestroyEffect", 2f);
	}

	public void DestroyEffect()
	{
		CancelInvoke();
		GroundEffectsManager.instance.RemoveGroundEffect(GetHashCode());
		ObjectPooler.instance.ReturnObjectToPool(base.gameObject, prefabName);
	}

	private void CalculateTime()
	{
		currentTime += Time.deltaTime;
		currentDuration += Time.deltaTime;
		if (currentTime >= tickTime)
		{
			ApplyTick();
			currentTime = 0f;
		}
		if (currentDuration >= duration)
		{
			EndEffect();
		}
	}

	private void Update()
	{
		if (isStart && RunManager.instance.isRunStarted)
		{
			CalculateTime();
		}
	}
}
public class GroundEffectsManager : MonoBehaviour
{
	public static GroundEffectsManager instance;

	private Dictionary<int, GroundEffectSelfer> AllGroundEffects = new Dictionary<int, GroundEffectSelfer>();

	private Dictionary<string, List<CustomFloatAndGO>> ChainEffects = new Dictionary<string, List<CustomFloatAndGO>>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}
		UnityEngine.Debug.Log("Destroyed Instance");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void StartRun()
	{
	}

	public void EndRun()
	{
		RemoveAllEffects();
	}

	public void SpawnChainEffect(string prefabName, Vector2 startPos, Vector2 endPos, float Duration)
	{
		GameObject gameObject = ObjectPooler.instance.GiveMeObject(prefabName, base.transform, base.transform.position);
		LineRenderer component = gameObject.GetComponent<LineRenderer>();
		if (component != null)
		{
			component.SetPositions(new Vector3[2] { startPos, endPos });
		}
		LineRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<LineRenderer>(includeInactive: true);
		foreach (LineRenderer lineRenderer in componentsInChildren)
		{
			if (lineRenderer != component)
			{
				lineRenderer.SetPositions(new Vector3[2] { startPos, endPos });
			}
		}
		if (!ChainEffects.ContainsKey(prefabName))
		{
			ChainEffects[prefabName] = new List<CustomFloatAndGO>();
		}
		ChainEffects[prefabName].Add(new CustomFloatAndGO(gameObject, Duration));
	}

	public void SpawnGround(string prefabName, Vector2 Pos, float radius, float duration, float tickTime, DamageData damage)
	{
		GroundEffectSelfer component = ObjectPooler.instance.GiveMeObject(prefabName, base.transform, Pos).GetComponent<GroundEffectSelfer>();
		component.TakeInfo(radius, duration, tickTime, damage);
		AllGroundEffects.Add(component.GetHashCode(), component);
	}

	public void RemoveGroundEffect(int HashCode)
	{
		AllGroundEffects.Remove(HashCode);
	}

	public void RemoveAllEffects()
	{
		List<int> list = AllGroundEffects.Keys.ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			AllGroundEffects[list[num]].DestroyEffect();
		}
		foreach (KeyValuePair<string, List<CustomFloatAndGO>> chainEffect in ChainEffects)
		{
			for (int num2 = chainEffect.Value.Count - 1; num2 >= 0; num2--)
			{
				ObjectPooler.instance.ReturnObjectToPool(chainEffect.Value[num2].TheGO, chainEffect.Key);
				chainEffect.Value.RemoveAt(num2);
			}
		}
	}

	public void DoReset()
	{
		RemoveAllEffects();
	}

	private void Update()
	{
		foreach (KeyValuePair<string, List<CustomFloatAndGO>> chainEffect in ChainEffects)
		{
			for (int num = chainEffect.Value.Count - 1; num >= 0; num--)
			{
				chainEffect.Value[num].TheFloat -= Time.deltaTime;
				if (chainEffect.Value[num].TheFloat <= 0f)
				{
					ObjectPooler.instance.ReturnObjectToPool(chainEffect.Value[num].TheGO, chainEffect.Key);
					chainEffect.Value.RemoveAt(num);
				}
			}
		}
	}
}
public class CustomFloatAndGO
{
	public GameObject TheGO;

	public float TheFloat;

	public CustomFloatAndGO(GameObject theGO, float theFloat)
	{
		TheGO = theGO;
		TheFloat = theFloat;
	}
}
public class LoggingManager : MonoBehaviour
{
	public static LoggingManager instance;

	private int StartingKilledMonstersCount;

	private int StartingShinyFoundCount;

	private float StartingTimePlayed;

	private double StartingGold;

	private string LogFilePath => Path.Combine(Application.persistentDataPath, "Logs.txt");

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}
		UnityEngine.Debug.Log("Destroyed Instance");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SaveLog_EndOfRun()
	{
	}

	public void SaveLog_ClearedLevel()
	{
	}

	private string SecondsToHoursMinutesSeconds(float seconds)
	{
		int num = (int)(seconds / 3600f);
		int num2 = (int)(seconds % 3600f / 60f);
		int num3 = (int)(seconds % 60f);
		if (num > 0)
		{
			return num + "h " + num2 + "m " + num3 + "s";
		}
		return num2 + "m " + num3 + "s";
	}
}
public class PSSelfer : MonoBehaviour
{
	private string MyName;

	private float Duration;

	private bool isint;

	private void OnEnable()
	{
		if (isint)
		{
			CancelInvoke();
			Invoke("DestroyMe", Duration);
		}
	}

	public void TakeInfo(string MyNameOnObjectPooler, float duration)
	{
		isint = true;
		CancelInvoke();
		Duration = duration;
		MyName = MyNameOnObjectPooler;
		Invoke("DestroyMe", Duration);
	}

	private void DestroyMe()
	{
		ObjectPooler.instance.ReturnObjectToPool(base.gameObject, MyName);
	}
}
public class RandomAudioSelfer : MonoBehaviour
{
	private AudioSource myAS;

	public Vector2 RandomInterval;

	private float currentPlayAfter;

	private void Start()
	{
		myAS = GetComponent<AudioSource>();
		currentPlayAfter = UnityEngine.Random.Range(RandomInterval.x, RandomInterval.y);
	}

	private void Update()
	{
		currentPlayAfter -= Time.deltaTime;
		if (currentPlayAfter <= 0f)
		{
			myAS.Play();
			currentPlayAfter = UnityEngine.Random.Range(RandomInterval.x, RandomInterval.y);
		}
	}
}
public class SoundSelfer : MonoBehaviour
{
	private float Duration;

	private bool isint;

	private AudioSource myAS;

	[HideInInspector]
	public string MyNameOnObjectPooler;

	public bool isIncreasePitchIfPlayedManyTimes;

	public bool isRandomizePitch = true;

	public float minPitch = 0.9f;

	public float maxPitch = 1.1f;

	private static Dictionary<string, int> numberOfTimesPlayed = new Dictionary<string, int>();

	private static Dictionary<string, float> lastTimePlayed = new Dictionary<string, float>();

	public List<AudioClip> RandomSelectAudioResources = new List<AudioClip>();

	private void OnEnable()
	{
		if (!numberOfTimesPlayed.ContainsKey(MyNameOnObjectPooler) && isIncreasePitchIfPlayedManyTimes)
		{
			numberOfTimesPlayed.Add(MyNameOnObjectPooler, 1);
			lastTimePlayed.Add(MyNameOnObjectPooler, Time.time);
		}
		if (!isint)
		{
			myAS = GetComponent<AudioSource>();
			isint = true;
			Duration = myAS.clip.length + 0.1f;
		}
		if (RandomSelectAudioResources.Count > 0)
		{
			myAS.clip = RandomSelectAudioResources[UnityEngine.Random.Range(0, RandomSelectAudioResources.Count)];
		}
		if (!isint)
		{
			return;
		}
		if (isRandomizePitch)
		{
			if (isIncreasePitchIfPlayedManyTimes)
			{
				float value = (minPitch + maxPitch) / 2f + (float)numberOfTimesPlayed[MyNameOnObjectPooler] * 0.015f;
				myAS.pitch = Mathf.Clamp(value, 0.5f, 2f);
				numberOfTimesPlayed[MyNameOnObjectPooler]++;
				if (Time.time - lastTimePlayed[MyNameOnObjectPooler] > 4f)
				{
					numberOfTimesPlayed[MyNameOnObjectPooler] = 0;
				}
				lastTimePlayed[MyNameOnObjectPooler] = Time.time;
				numberOfTimesPlayed[MyNameOnObjectPooler]++;
			}
			else
			{
				myAS.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
			}
		}
		myAS.Play();
		CancelInvoke();
		Invoke("DestroyMe", Duration);
	}

	private void DestroyMe()
	{
		if (FXManager.instance != null && !string.IsNullOrEmpty(MyNameOnObjectPooler))
		{
			FXManager.instance.OnSoundFinished(MyNameOnObjectPooler);
		}
		ObjectPooler.instance.ReturnObjectToPool(base.gameObject, MyNameOnObjectPooler, IsDestroy_UsedForObjectsThatAreRarelyUsed: true);
	}
}
public class TextSelfFloating : MonoBehaviour
{
	[HideInInspector]
	public TextMeshProUGUI MyTextMeshPro;

	private Transform myTransform;

	public float WaitSecBeforeDestroy = 1.5f;

	public string MyObjectPoolKey;

	[Header("FLYFF")]
	public float MaxScaleAfterMovement;

	public float ScaleDuration;

	public float YFinal;

	public Vector2 XFromTo;

	public float YMovingDuration;

	public float FadingDuration;

	public float WaitAfterMovingY;

	public float RandomFrom;

	public float RandomTo;

	private Color TextsColor;

	[HideInInspector]
	public Color OriginalColor;

	private float baseFontSize;

	[HideInInspector]
	public bool isInit;

	private float RandScalingDuration;

	private float RandScalingY;

	private void DestroyFloater()
	{
		if (MyObjectPoolKey == "FloatingGold")
		{
			FloatingNumbersManager.instance.HowManyActive_GoldNumbers--;
		}
		else
		{
			FloatingNumbersManager.instance.HowManyActive_DamageNumbers--;
		}
		MyTextMeshPro.text = "";
		CancelInvoke();
		ObjectPooler.instance.ReturnObjectToPool(base.gameObject, MyObjectPoolKey);
	}

	public void TakeInfo(string Number)
	{
		if (!isInit)
		{
			MyTextMeshPro = GetComponent<TextMeshProUGUI>();
			myTransform = base.transform;
			baseFontSize = MyTextMeshPro.fontSize;
			OriginalColor = MyTextMeshPro.color;
			isInit = true;
		}
		myTransform.localScale = new Vector3(1f, 1f, 1f);
		MyTextMeshPro.text = Number;
		TextsColor = OriginalColor;
		TextsColor.a = 1f;
		MyTextMeshPro.color = TextsColor;
		Invoke("DestroyFloater", WaitSecBeforeDestroy);
		RandScalingDuration = UnityEngine.Random.Range(RandomFrom * YMovingDuration, RandomTo * YMovingDuration);
		RandScalingY = UnityEngine.Random.Range(RandomFrom * YFinal, RandomTo * YFinal);
		DG.Tweening.Sequence s = DOTween.Sequence();
		s.Append(myTransform.DOLocalMoveY(myTransform.localPosition.y + RandScalingY, RandScalingDuration).SetEase(Ease.OutCubic));
		if (Mathf.Abs(XFromTo.x - XFromTo.y) > 0.1f)
		{
			s.Join(myTransform.DOLocalMoveX(myTransform.localPosition.x + UnityEngine.Random.Range(XFromTo.x, XFromTo.y), RandScalingDuration).SetEase(Ease.OutCubic));
		}
		s.Append(MyTextMeshPro.DOFade(0f, FadingDuration).SetDelay(WaitAfterMovingY));
		s.Join(myTransform.DOScale(MaxScaleAfterMovement, ScaleDuration));
	}

	public IEnumerator TakeInfo_HideThenShow(string Number)
	{
		if (MyTextMeshPro == null)
		{
			MyTextMeshPro = GetComponent<TextMeshProUGUI>();
			myTransform = base.transform;
		}
		myTransform.localScale = Vector3.zero;
		yield return new WaitForSeconds(0.5f);
		myTransform.localScale = new Vector3(1f, 1f, 1f);
		TextsColor = MyTextMeshPro.color;
		TextsColor.a = 1f;
		MyTextMeshPro.color = TextsColor;
		MyTextMeshPro.text = Number;
		Invoke("DestroyFloater", WaitSecBeforeDestroy);
		RandScalingDuration = UnityEngine.Random.Range(RandomFrom * YMovingDuration, RandomTo * YMovingDuration);
		RandScalingY = UnityEngine.Random.Range(RandomFrom * YFinal, RandomTo * YFinal);
		DG.Tweening.Sequence s = DOTween.Sequence();
		s.Append(myTransform.DOLocalMoveY(myTransform.localPosition.y + RandScalingY, RandScalingDuration).SetEase(Ease.OutCubic));
		s.Append(MyTextMeshPro.DOFade(0f, FadingDuration).SetDelay(WaitAfterMovingY));
		s.Join(myTransform.DOScale(MaxScaleAfterMovement, ScaleDuration));
	}
}
public class TrailAddMultiController : MonoBehaviour
{
	private Vector3 startPosition;

	private Vector3 targetPosition;

	private float moveSpeed;

	private float journeyLength;

	private float startTime;

	private float targetTimeToReach;

	private bool isMoving;

	private Vector3 controlPoint;

	private TrailRenderer trailRenderer;

	public void TakeInfo(Vector3 startPos, Vector3 targetPos)
	{
		startPosition = startPos;
		targetPosition = targetPos;
		targetPosition.z = startPosition.z;
		journeyLength = Vector3.Distance(startPosition, targetPosition);
		startTime = Time.time;
		isMoving = true;
		trailRenderer = GetComponent<TrailRenderer>();
		trailRenderer.Clear();
		targetTimeToReach = 0.9f;
		if (journeyLength < 200f)
		{
			targetTimeToReach *= journeyLength / 600f;
		}
		moveSpeed = journeyLength / targetTimeToReach;
		Vector3 normalized = (targetPosition - startPosition).normalized;
		Vector3 vector = new Vector3(0f - normalized.y, normalized.x, 0f);
		Vector3 vector2 = new Vector3(normalized.y, 0f - normalized.x, 0f);
		float num = Mathf.Atan2(vector.y, vector.x);
		float num2 = Mathf.Atan2(vector2.y, vector2.x);
		if (num2 < num)
		{
			num2 += MathF.PI * 2f;
		}
		float f = UnityEngine.Random.Range(num, num2);
		Vector3 vector3 = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
		float num3 = journeyLength * 0.5f;
		controlPoint = startPosition + vector3 * num3;
	}

	private float EaseInOutQuad(float x)
	{
		if (!(x < 0.5f))
		{
			return 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
		}
		return 2f * x * x;
	}

	private void FixedUpdate()
	{
		if (!isMoving)
		{
			return;
		}
		float num = (Time.time - startTime) * moveSpeed / journeyLength;
		float t = EaseInOutQuad(Mathf.Clamp01(num));
		Vector3 a = Vector3.Lerp(startPosition, controlPoint, t);
		Vector3 b = Vector3.Lerp(controlPoint, targetPosition, t);
		Vector3 position = Vector3.Lerp(a, b, t);
		base.transform.position = position;
		if (num >= 1f)
		{
			isMoving = false;
			if (FXManager.instance.SpawnGFX("WellTrailEffect", targetPosition, 1f, Vector3.one) != null && FunctionsNeeded.IsHappened(30f))
			{
				FXManager.instance.PlaySound("Well_HitSound");
			}
			WellManager.instance.BounceWell();
		}
	}
}
public class DebuffManager : MonoBehaviour
{
	public static DebuffManager instance;

	private float DebuffTimer;

	private Dictionary<string, float> DebuffChances_Base = new Dictionary<string, float>();

	private Dictionary<string, float> DebuffChances = new Dictionary<string, float>();

	private float ShinyDebuffChance_BasePercentage = 1.1f;

	private float SizeToGameUnitRatio = 130f;

	private bool isJustUnlockedDebuff;

	public Color DamageTakenDebuffColor;

	public Color GoldDroppedDebuffColor;

	public Color CallsSkillDebuffColor;

	public Color SpawnShinyDebuffColor;

	private float UnluckyProtectionTotalForDebuffs;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		isJustUnlockedDebuff = false;
		DebuffChances_Base.Add("DebuffFX_DamageTaken", 200f);
		DebuffChances_Base.Add("DebuffFX_GoldDropped", 150f);
		DebuffChances_Base.Add("DebuffFX_CallsSkill", 40f);
		DebuffChances_Base.Add("DebuffFX_SpawnShiny", DebuffChances_Base.Values.Sum() / (100f / ShinyDebuffChance_BasePercentage - 1f));
	}

	private Color GetDebuffColor(string debuffName)
	{
		return debuffName switch
		{
			"DebuffFX_DamageTaken" => DamageTakenDebuffColor, 
			"DebuffFX_GoldDropped" => GoldDroppedDebuffColor, 
			"DebuffFX_CallsSkill" => CallsSkillDebuffColor, 
			"DebuffFX_SpawnShiny" => SpawnShinyDebuffColor, 
			_ => Color.white, 
		};
	}

	public void StartRun()
	{
		DebuffTimer = DatabaseManager.BaseDebuffCheckEverySecond;
		DebuffChances = new Dictionary<string, float>();
		UnluckyProtectionTotalForDebuffs = 50f;
		if (playerData.instance.stats.Debuff_DamageTaken_CanBeFoundInRun.Total.RealValue > 50)
		{
			if (!isJustUnlockedDebuff)
			{
				isJustUnlockedDebuff = true;
			}
			DebuffChances.Add("DebuffFX_DamageTaken", DebuffChances_Base["DebuffFX_DamageTaken"]);
		}
		if (playerData.instance.stats.Debuff_GoldDropped_CanBeFoundInRun.Total.RealValue > 50)
		{
			DebuffChances.Add("DebuffFX_GoldDropped", DebuffChances_Base["DebuffFX_GoldDropped"]);
		}
		if (playerData.instance.stats.Debuff_CallSkills_CanBeFoundInRun.Total.RealValue > 50)
		{
			DebuffChances.Add("DebuffFX_CallsSkill", DebuffChances_Base["DebuffFX_CallsSkill"]);
		}
		if (playerData.instance.stats.Debuff_SpawnShiny_CanBeFoundInRun.Total.RealValue > 50)
		{
			DebuffChances.Add("DebuffFX_SpawnShiny", DebuffChances.Values.Sum() / (100f / ShinyDebuffChance_BasePercentage - 1f));
		}
	}

	public void EndRun()
	{
	}

	public void ApplyDebuffOnEnemy(EnemySelfer enemy, string debuffName)
	{
		enemy.AddDebuff(debuffName);
	}

	public void ApplyRandomDebuffOnEnemy(EnemySelfer enemy)
	{
		if (!(enemy == null) && !enemy.isDead)
		{
			string aRandomFromDict_Normal = FunctionsNeeded.GetARandomFromDict_Normal(DebuffChances);
			enemy.AddDebuff(aRandomFromDict_Normal);
		}
	}

	private void ApplyRandomDebuffOnRandomArea()
	{
		string aRandomFromDict_Normal = FunctionsNeeded.GetARandomFromDict_Normal(DebuffChances);
		EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
		if (randomEnemy == null)
		{
			return;
		}
		playerData.instance.TotalAreaMarksApplied_CurrentRun++;
		Vector2 vector = randomEnemy.GetPosition();
		float num = playerData.instance.stats.DebuffRadius.Total.RealValue * DatabaseManager.OneGameUnitToUnityUnit;
		if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceForDebuffToTargetDenseAreas.Total.RealValue) || playerData.instance.MonstersLevel < 11)
		{
			vector = GetDensePosition(num);
		}
		if (FXManager.instance.SpawnGFX(aRandomFromDict_Normal + "_Area", vector, 1.5f, Vector3.one * num / SizeToGameUnitRatio) != null)
		{
			FXManager.instance.PlaySound("Debuff_FireSound", ForcePlay: true);
		}
		List<EnemySelfer> enemiesInCircle = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(vector, num);
		foreach (EnemySelfer item in enemiesInCircle)
		{
			ApplyDebuffOnEnemy(item, aRandomFromDict_Normal);
		}
		RunManager.instance.ChangeTimer(playerData.instance.stats.TimeGainedWheneverAMonsterIsDebuffed.Total.RealValue * (float)enemiesInCircle.Count);
	}

	private Vector2 GetDensePosition(float radius)
	{
		List<Vector2> list = EnemiesManager.instance.SampleEvenlySpacedPointsInTrapezoid(100);
		list.AddRange(EnemiesManager.instance.AliveEnemies_Transform.Values.Select((Transform x) => new Vector2(x.position.x, x.position.y)));
		Vector2 result = Vector2.zero;
		int num = 0;
		foreach (Vector2 item in list)
		{
			int count = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(item, radius).Count;
			if (count > num)
			{
				num = count;
				result = item;
			}
		}
		return result;
	}

	private void Update()
	{
		if (!RunManager.instance.isRunStarted || DebuffChances.Count <= 0)
		{
			return;
		}
		DebuffTimer -= Time.deltaTime;
		if (DebuffTimer <= 0f)
		{
			DebuffTimer = DatabaseManager.BaseDebuffCheckEverySecond;
			if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceToSpawnDebuff.Total.RealValue + UnluckyProtectionTotalForDebuffs) || isJustUnlockedDebuff)
			{
				isJustUnlockedDebuff = false;
				ApplyRandomDebuffOnRandomArea();
				UnluckyProtectionTotalForDebuffs = 0f;
			}
			else if (playerData.instance.stats.ChanceToSpawnDebuff.Total.RealValue <= 20f)
			{
				UnluckyProtectionTotalForDebuffs += 5f;
			}
			else if (playerData.instance.stats.ChanceToSpawnDebuff.Total.RealValue <= 40f)
			{
				UnluckyProtectionTotalForDebuffs += 3f;
			}
			else if (playerData.instance.stats.ChanceToSpawnDebuff.Total.RealValue <= 60f)
			{
				UnluckyProtectionTotalForDebuffs += 2f;
			}
		}
	}
}
public class MouseAttacker : MonoBehaviour
{
	public static MouseAttacker instance;

	public GameObject MouseIndicator;

	public Transform FullSizeTransform;

	public Transform ExpanderTransform;

	private float SizeToGameUnitRatio = 60f;

	private float FullSizeValue;

	public ProjectileInfo mouseProjectile;

	public ProjectileInfo mouseRagedProjectile;

	private List<MouseAttack> MouseAttacks = new List<MouseAttack>();

	private float idleLaunchTimer;

	[HideInInspector]
	public Color DefaultFullSizeColor;

	[HideInInspector]
	public Color DefaultExpanderColor;

	public Color RagedFullSizeColor;

	public Color RagedExpanderColor;

	private int HowManyTimesHitSoundPlayed;

	private Dictionary<int, float> ChanceToPlayHitSound = new Dictionary<int, float>
	{
		{ 30, 100f },
		{ 100, 65f },
		{ 200, 40f },
		{
			int.MaxValue,
			25f
		}
	};

	private List<int> ChancesThresholdToPlayHitSound = new List<int>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		MouseAttacks.Add(new MouseAttack(MouseIndicator, FullSizeTransform, ExpanderTransform));
		DefaultFullSizeColor = FullSizeTransform.GetComponent<SpriteRenderer>().color;
		DefaultExpanderColor = ExpanderTransform.GetComponent<SpriteRenderer>().color;
		MouseAttacks[0].SetRaged(isRaged: false);
		ChancesThresholdToPlayHitSound = new List<int>(ChanceToPlayHitSound.Keys);
	}

	public void StartRun()
	{
		idleLaunchTimer = 0f;
		MouseAttacks[0].attackTimer = 0f;
		MouseAttacks[0].MouseIndicator.SetActive(value: true);
		MouseAttacks[0].SetRaged(isRaged: false);
		ChangeMouseSize(0);
		HowManyTimesHitSoundPlayed = 0;
	}

	public void EndRun()
	{
		for (int num = MouseAttacks.Count - 1; num > 0; num--)
		{
			RemoveAMouseAttack(num);
		}
		MouseAttacks[0].CleanupAnimation();
		MouseAttacks[0].MouseIndicator.SetActive(value: false);
	}

	public void ChangeMouseSize(int index)
	{
		FullSizeValue = playerData.instance.stats.MouseRadius.Total.RealValue * DatabaseManager.OneGameUnitToUnityUnit / SizeToGameUnitRatio;
		if (MouseAttacks[index].isRaged)
		{
			MouseAttacks[index].FullSizeTransform.localScale = Vector3.zero;
			float realValue = playerData.instance.stats.RagedMouseAreaMultiplier.Total.RealValue;
			MouseAttacks[index].FullSizeTransform.DOScale(FullSizeValue * realValue, 0.25f);
		}
		else
		{
			MouseAttacks[index].FullSizeTransform.DOScale(FullSizeValue, 0.25f);
		}
	}

	private void MouseAttackFunction(int index)
	{
		MouseAttacks[index].attackTimer += Time.deltaTime;
		float num = 1f;
		if (MouseAttacks[index].isRaged)
		{
			num = playerData.instance.stats.RagedMouseAreaMultiplier.Total.RealValue;
		}
		float num2 = Mathf.Lerp(0f, FullSizeValue * num, MouseAttacks[index].attackTimer / (1f / playerData.instance.stats.MouseAttackSpeed.Total.RealValue));
		MouseAttacks[index].ExpanderTransform.localScale = Vector3.one * num2;
		if (!(MouseAttacks[index].attackTimer >= 1f / playerData.instance.stats.MouseAttackSpeed.Total.RealValue))
		{
			return;
		}
		MouseAttacks[index].attackTimer = 0f;
		MouseAttacks[index].ExpanderTransform.localScale = Vector3.zero;
		playerData.instance.TotalMouseAttacks_FullGame++;
		StartCoroutine(DealDamage(index));
		bool flag = false;
		if (RunManager.instance.isRunStarted && index > 0 && MouseAttacks.Count > index)
		{
			MouseAttacks[index].TriggerCount++;
			if (MouseAttacks[index].TriggerCount >= playerData.instance.stats.TriggersOfIdleMouseAttacks.Total.RealValue)
			{
				RemoveAMouseAttack(index);
				flag = true;
			}
		}
		if (!flag && RunManager.instance.isRunStarted)
		{
			MouseAttacks[index].SetRaged(isRaged: false);
			if (index == 0 && FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceForMouseAttackToBeRaged.Total.RealValue))
			{
				FXManager.instance.PlaySound("RageFireSound", ForcePlay: true);
				MouseAttacks[index].SetRaged(isRaged: true);
			}
			else if (index > 0 && FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceForIdleMouseAttackToBeRaged.Total.RealValue))
			{
				MouseAttacks[index].SetRaged(isRaged: true);
			}
			ChangeMouseSize(index);
		}
	}

	public bool IsForgiveTimer()
	{
		if (playerData.instance.MonstersLevel <= 2 && MouseAttacks[0].attackTimer / (1f / playerData.instance.stats.MouseAttackSpeed.Total.RealValue) > 0.7f)
		{
			return true;
		}
		return false;
	}

	private IEnumerator DealDamage(int index)
	{
		float num = 1f;
		bool isRaged = false;
		if (MouseAttacks[index].isRaged)
		{
			num = playerData.instance.stats.RagedMouseAreaMultiplier.Total.RealValue;
			isRaged = true;
		}
		List<EnemySelfer> enemies = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(MouseAttacks[index].MouseIndicator.transform.position, playerData.instance.stats.MouseRadius.Total.RealValue * DatabaseManager.OneGameUnitToUnityUnit * num);
		int iter = 0;
		bool isTwiceHit = false;
		float num2 = playerData.instance.stats.ChanceForTwiceHits.Total.RealValue;
		if (num2 > 1f && playerData.instance.MonstersLevel == 1)
		{
			num2 = 25f;
		}
		if (FunctionsNeeded.IsHappened(num2))
		{
			isTwiceHit = true;
		}
		int howManyProjectiles = 0;
		DamageData dd = playerData.instance.stats.DamageCalculation("mouse", "damage1", playerData.instance.stats.MouseAttack_DamageMultiplier.Total.RealValue * ((isRaged && playerData.instance.stats.RagedMouseDamageMultiplier.Total.RealValue >= 1f) ? playerData.instance.stats.RagedMouseDamageMultiplier.Total.RealValue : 1f));
		bool isShowEffect = FunctionsNeeded.IsHappened(playerData.instance.EffectsAmount);
		if (index == 0)
		{
			isShowEffect = true;
			if (enemies.Count > 0)
			{
				FXManager.instance.PlaySound("MouseAttackFireSound", isShowEffect);
			}
			else
			{
				FXManager.instance.PlaySound("MouseAttackFireSound_NoHit", isShowEffect);
			}
		}
		foreach (EnemySelfer item in enemies)
		{
			if (isTwiceHit)
			{
				StartCoroutine(TwiceHit(item, dd, isShowEffect, index == 0));
			}
			else
			{
				item.TakeDamage(dd, isOriginalHit: true, isMouseAttack: true, index == 0);
				FXManager.instance.SpawnGFX("MouseHitEffect", item.GetPosition(), 2f, Vector3.zero, 0f, isShowEffect);
			}
			if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceToFireMouseProjectile.Total.RealValue))
			{
				if (!RunManager.instance.isRunStarted || howManyProjectiles >= 5)
				{
					continue;
				}
				howManyProjectiles++;
				if (isPlayHitSound())
				{
					FXManager.instance.PlaySound(mouseProjectile.functionName + "_FireSound");
				}
				HowManyTimesHitSoundPlayed++;
				EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
				if (isRaged && FunctionsNeeded.IsHappened(playerData.instance.stats.RagedMouseChanceToFireRagedProjectile.Total.RealValue))
				{
					ProjectilesManager.instance.FireProjectile(item.transform.position, randomEnemy.transform.position, mouseRagedProjectile, MultipleProjectileFormation.GMP, 1 + playerData.instance.stats.MouseProjectile_AdditionalProjectiles.Total.RealValue, 50f, isFiredFromTowerDirectly: true);
				}
				else
				{
					ProjectilesManager.instance.FireProjectile(item.transform.position, randomEnemy.transform.position, mouseProjectile, MultipleProjectileFormation.GMP, 1 + playerData.instance.stats.MouseProjectile_AdditionalProjectiles.Total.RealValue, 50f, isFiredFromTowerDirectly: true);
				}
			}
			iter++;
			if (enemies.Count <= 4 && RunManager.instance.isRunStarted)
			{
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				yield return null;
			}
		}
		AutomatorBot.instance.MouseDealtAttack();
	}

	private IEnumerator TwiceHit(EnemySelfer enemy, DamageData dd, bool isShowEffect, bool isForceSound = false)
	{
		enemy.TakeDamage(dd, isOriginalHit: true, isMouseAttack: true, isForceSound);
		FXManager.instance.SpawnGFX("MouseHitEffect", enemy.GetPosition(), 2f, Vector3.zero, 0f, isShowEffect);
		yield return new WaitForSeconds(0.2f);
		if (RunManager.instance.isRunStarted)
		{
			enemy.TakeDamage(dd, isOriginalHit: false, isMouseAttack: true, isForceSound);
			FXManager.instance.SpawnGFX("MouseHitEffect", enemy.GetPosition(), 2f, Vector3.zero, 0f, isShowEffect);
		}
	}

	private bool isPlayHitSound()
	{
		int num = -1;
		for (int i = 0; i < ChancesThresholdToPlayHitSound.Count; i++)
		{
			if (HowManyTimesHitSoundPlayed < ChancesThresholdToPlayHitSound[i])
			{
				num = ChancesThresholdToPlayHitSound[i];
				break;
			}
		}
		if (num == -1)
		{
			return true;
		}
		return FunctionsNeeded.IsHappened(ChanceToPlayHitSound[num]);
	}

	private void RemoveAMouseAttack(int index)
	{
		MouseAttacks[index].CleanupAnimation();
		MouseAttacks[index].MouseIndicator.SetActive(value: false);
		ObjectPooler.instance.ReturnObjectToPool(MouseAttacks[index].MouseIndicator, "MouseIndicatorPrefab");
		MouseAttacks.RemoveAt(index);
	}

	private void LaunchNewMouseAttack(Vector3 Position)
	{
		GameObject gameObject = ObjectPooler.instance.GiveMeObject("MouseIndicatorPrefab", base.transform, Position);
		MouseAttacks.Add(new MouseAttack(gameObject, gameObject.transform.Find("FullSize"), gameObject.transform.Find("Expander")));
		MouseAttacks[MouseAttacks.Count - 1].MouseIndicator.transform.position = Position;
		MouseAttacks[MouseAttacks.Count - 1].MouseIndicator.SetActive(value: true);
		MouseAttacks[MouseAttacks.Count - 1].SetRaged(isRaged: false);
		if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceForIdleMouseAttackToBeRaged.Total.RealValue))
		{
			MouseAttacks[MouseAttacks.Count - 1].SetRaged(isRaged: true);
			gameObject.GetComponent<Breather>().enabled = false;
		}
		else
		{
			gameObject.GetComponent<Breather>().enabled = true;
		}
		ChangeMouseSize(MouseAttacks.Count - 1);
	}

	private void Update()
	{
		if (!RunManager.instance.isRunStarted)
		{
			return;
		}
		Vector3 position = FunctionsNeeded.MouseWorldPosition(0f);
		position.z = 0f;
		MouseAttacks[0].MouseIndicator.transform.position = position;
		if (playerData.instance.stats.IdleMouseAttackCooldown.Total.RealValue > 0.1f)
		{
			idleLaunchTimer += Time.deltaTime;
			if (idleLaunchTimer >= playerData.instance.stats.IdleMouseAttackCooldown.Total.RealValue)
			{
				for (int i = 0; i < playerData.instance.stats.NumberOfIdleMouseAttacks.Total.RealValue; i++)
				{
					EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
					if (randomEnemy != null)
					{
						LaunchNewMouseAttack(randomEnemy.transform.position);
					}
				}
				idleLaunchTimer = 0f;
			}
		}
		for (int num = MouseAttacks.Count - 1; num >= 0; num--)
		{
			MouseAttackFunction(num);
		}
	}
}
public class MouseAttack
{
	public GameObject MouseIndicator;

	private Transform MouseIndicatorTransform;

	public Transform FullSizeTransform;

	public Transform ExpanderTransform;

	public SpriteRenderer FullSizeSpriteRenderer;

	public SpriteRenderer ExpanderSpriteRenderer;

	public float attackTimer;

	public int TriggerCount;

	public bool isRaged;

	private DG.Tweening.Sequence ragedAnimation;

	private float originalScale;

	public MouseAttack(GameObject MouseIndicatorGO, Transform FullSizeTransform, Transform ExpanderTransform)
	{
		MouseIndicator = MouseIndicatorGO;
		MouseIndicatorTransform = MouseIndicatorGO.transform;
		this.ExpanderTransform = ExpanderTransform;
		this.FullSizeTransform = FullSizeTransform;
		attackTimer = 0f;
		TriggerCount = 0;
		FullSizeSpriteRenderer = FullSizeTransform.GetComponent<SpriteRenderer>();
		ExpanderSpriteRenderer = ExpanderTransform.GetComponent<SpriteRenderer>();
		isRaged = false;
		originalScale = 1f;
	}

	public void SetRaged(bool isRaged)
	{
		this.isRaged = isRaged;
		if (ragedAnimation != null)
		{
			ragedAnimation.Kill();
			ragedAnimation = null;
		}
		if (isRaged)
		{
			FullSizeSpriteRenderer.color = MouseAttacker.instance.RagedFullSizeColor;
			ExpanderSpriteRenderer.color = MouseAttacker.instance.RagedExpanderColor;
			ragedAnimation = DOTween.Sequence().Append(MouseIndicatorTransform.DOScale(originalScale * 1.025f, 0.1f)).Append(MouseIndicatorTransform.DOScale(originalScale * 0.975f, 0.1f))
				.SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			FullSizeSpriteRenderer.color = MouseAttacker.instance.DefaultFullSizeColor;
			ExpanderSpriteRenderer.color = MouseAttacker.instance.DefaultExpanderColor;
		}
	}

	public void CleanupAnimation()
	{
		if (ragedAnimation != null)
		{
			ragedAnimation.Kill();
			ragedAnimation = null;
		}
		MouseIndicatorTransform.localScale = Vector3.one * originalScale;
	}
}
public class PlayerManager : MonoBehaviour
{
	public static PlayerManager instance;

	public Dictionary<Currencies, Action> OnCurrencyChange = new Dictionary<Currencies, Action>();

	public BarSelfer LevelBar;

	public TextMeshProUGUI LevelText;

	public BarSelfer LevelBar_NonRun;

	public TextMeshProUGUI LevelText_NonRun;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		foreach (Currencies value in Enum.GetValues(typeof(Currencies)))
		{
			OnCurrencyChange.Add(value, delegate
			{
			});
		}
		UpdateLevelUI();
	}

	public void StartRun()
	{
		UpdateLevelUI();
	}

	public void EndRun()
	{
	}

	public void TriggerEventsFromOutside(Currencies currency)
	{
		OnCurrencyChange[currency]?.Invoke();
	}

	public void ChangeCurrency(Currencies currency, double Amount, bool isApplyGainMulti = true, bool isApplyAch = true)
	{
		switch (currency)
		{
		case Currencies.Gold:
			Amount *= (double)((!isApplyGainMulti) ? 1f : ((Amount > 0.0) ? playerData.instance.stats.GoldGained.Total.RealValue : 1f));
			playerData.instance.PlayerGold += Math.Round(Amount);
			if (!(Amount > 0.0))
			{
			}
			break;
		case Currencies.LevelPoints:
			playerData.instance.LevelPoints += (int)Math.Round(Amount);
			break;
		case Currencies.CharacterCurrency:
			playerData.instance.CharacterCurrency += (int)Math.Round(Amount);
			break;
		case Currencies.GemCurrency:
			playerData.instance.GemCurrency += (int)Math.Round(Amount);
			break;
		case Currencies.ClearCurrency:
			playerData.instance.ClearCurrency += (int)Math.Round(Amount);
			if (playerData.instance.TotalCurrenciesGained_FullGame[Currencies.ClearCurrency] >= 39.0 && playerData.instance.ClearCurrency == 0)
			{
				AchievementsManager.instance.UnlockAchievement("TreeFull_Unlocked");
			}
			break;
		case Currencies.WellCurrency:
			playerData.instance.WellCurrency += (int)Math.Round(Amount);
			break;
		}
		OnCurrencyChange[currency]?.Invoke();
	}

	public bool IsCanSpendCurrency(Currencies currency, double Amount, bool IsSpendAlso)
	{
		switch (currency)
		{
		case Currencies.Gold:
			if (FunctionsNeeded.IsDoubleALargerOrEqualB(playerData.instance.PlayerGold, Amount))
			{
				if (IsSpendAlso)
				{
					ChangeCurrency(currency, 0.0 - Amount);
				}
				return true;
			}
			return false;
		case Currencies.LevelPoints:
			if (FunctionsNeeded.IsDoubleALargerOrEqualB(playerData.instance.LevelPoints, Amount))
			{
				if (IsSpendAlso)
				{
					ChangeCurrency(currency, 0.0 - Amount);
				}
				return true;
			}
			return false;
		case Currencies.CharacterCurrency:
			if (FunctionsNeeded.IsDoubleALargerOrEqualB(playerData.instance.CharacterCurrency, Amount))
			{
				if (IsSpendAlso)
				{
					ChangeCurrency(currency, 0.0 - Amount);
				}
				return true;
			}
			return false;
		case Currencies.GemCurrency:
			if (FunctionsNeeded.IsDoubleALargerOrEqualB(playerData.instance.GemCurrency, Amount))
			{
				if (IsSpendAlso)
				{
					ChangeCurrency(currency, 0.0 - Amount);
				}
				return true;
			}
			return false;
		case Currencies.ClearCurrency:
			if (FunctionsNeeded.IsDoubleALargerOrEqualB(playerData.instance.ClearCurrency, Amount))
			{
				if (IsSpendAlso)
				{
					ChangeCurrency(currency, 0.0 - Amount);
				}
				return true;
			}
			return false;
		case Currencies.WellCurrency:
			if (FunctionsNeeded.IsDoubleALargerOrEqualB(playerData.instance.WellCurrency, Amount))
			{
				if (IsSpendAlso)
				{
					ChangeCurrency(currency, 0.0 - Amount);
				}
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public (bool, List<bool>) IsCanSpendCurrencies(List<double> costs, List<Currencies> currencies, bool isSpendAlso)
	{
		List<bool> list = new List<bool>();
		for (int i = 0; i < costs.Count; i++)
		{
			list.Add(IsCanSpendCurrency(currencies[i], costs[i], IsSpendAlso: false));
		}
		bool flag = list.All((bool x) => x);
		if (flag && isSpendAlso)
		{
			for (int j = 0; j < costs.Count; j++)
			{
				ChangeCurrency(currencies[j], 0.0 - costs[j]);
			}
		}
		return (flag, list);
	}

	public void MonsterDiedGiveRewards(Vector3 Position, EnemyType enemyType, int SkinID, bool isDebuffed_GoldDropped, bool isBoss)
	{
		if (RunManager.instance.IsBossRun)
		{
			SkinID = 0;
		}
		if (enemyType == EnemyType.Slime && SkinID > 0)
		{
			ShinyInfo shinyInfo = DatabaseManager.ShinyDict_SkinID[SkinID];
			if (playerData.instance.ShinyCounts[shinyInfo.FunctionName] < shinyInfo.MaxLevel)
			{
				ShinyManager.instance.GainedAShiny(shinyInfo);
				playerData.instance.TotalShinyFound_CurrentRun[shinyInfo.FunctionName]++;
				playerData.instance.TotalShinyFound_FullGame[shinyInfo.FunctionName]++;
				playerData.instance.TotalShinyFound++;
				if (RunManager.instance.isDoubleLootRun && playerData.instance.ShinyCounts[shinyInfo.FunctionName] < shinyInfo.MaxLevel)
				{
					ShinyManager.instance.GainedAShiny(shinyInfo);
					playerData.instance.TotalShinyFound_CurrentRun[shinyInfo.FunctionName]++;
					playerData.instance.TotalShinyFound_FullGame[shinyInfo.FunctionName]++;
					playerData.instance.TotalShinyFound++;
				}
			}
			AchievementsManager.instance.UnlockAchievement("ShinyOne");
			bool flag = true;
			for (int i = 0; i < DatabaseManager.ShinyList.Count; i++)
			{
				if (playerData.instance.ShinyCounts[DatabaseManager.ShinyList[i].FunctionName] == 0)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				AchievementsManager.instance.UnlockAchievement("ShinyAll");
			}
		}
		WellManager.instance.MonsterDiedGainExp();
		double num = DatabaseManager.EnemyExpDrop(playerData.instance.MonstersLevel);
		num *= (double)playerData.instance.stats.ExpGained.Total.RealValue;
		if (enemyType == EnemyType.TreasureChest && SkinID == 1)
		{
			num *= (double)playerData.instance.stats.TreasureExpMultiplier.Total.RealValue;
		}
		GainExp(num);
		double num2 = DatabaseManager.EnemyGold(playerData.instance.MonstersLevel);
		num2 *= (double)playerData.instance.stats.GoldGained.Total.RealValue;
		if (enemyType == EnemyType.TreasureChest && SkinID == 0)
		{
			num2 *= (double)playerData.instance.stats.TreasureGoldMultiplier.Total.RealValue;
		}
		if (isDebuffed_GoldDropped)
		{
			num2 *= (double)playerData.instance.stats.Debuff_GoldMultiplier.Total.RealValue;
		}
		if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceToManyXGoldGained.Total.RealValue))
		{
			num2 *= (double)DatabaseManager.XAmountToManyXGoldGained;
		}
		string text = num2.ToReadable();
		if (RunManager.instance.isDoubleGoldRun && RunManager.instance.isDoubleLootRun)
		{
			num2 *= 4.0;
		}
		else if (RunManager.instance.isDoubleGoldRun || RunManager.instance.isDoubleLootRun)
		{
			num2 *= 2.0;
		}
		playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.Gold] += num2;
		playerData.instance.TotalCurrenciesGained_FullGame[Currencies.Gold] += num2;
		ChangeCurrency(Currencies.Gold, num2, isApplyGainMulti: false);
		FloatingNumbersManager.instance.GenerateFloatingNumber("<sprite name=Gold>" + text, Position, "GoldFloater");
		switch (enemyType)
		{
		case EnemyType.Ghost:
		{
			int num4 = playerData.instance.stats.CharacterCurrencyDrop.Total.RealValue;
			if (isBoss)
			{
				num4 *= 3;
			}
			string text3 = num4.ToString("0");
			if (RunManager.instance.isDoubleLootRun)
			{
				num4 *= 2;
			}
			ChangeCurrency(Currencies.CharacterCurrency, num4);
			FloatingNumbersManager.instance.GenerateFloatingNumber("<sprite name=CharacterCurrency>" + text3, Position, "GoldFloater");
			playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.CharacterCurrency] += num4;
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.CharacterCurrency] += num4;
			break;
		}
		case EnemyType.Ore:
		{
			int num3 = Mathf.FloorToInt((float)playerData.instance.stats.OreDrop_GemCurrency.Total.RealValue * ((SkinID == 0) ? 1f : playerData.instance.stats.RichOreMultiplier.Total.RealValue));
			string text2 = num3.ToString("0");
			if (RunManager.instance.isDoubleLootRun)
			{
				num3 *= 2;
			}
			ChangeCurrency(Currencies.GemCurrency, num3);
			FloatingNumbersManager.instance.GenerateFloatingNumber("<sprite name=GemCurrency>" + text2, Position, "GoldFloater");
			playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.GemCurrency] += num3;
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.GemCurrency] += num3;
			break;
		}
		}
	}

	public void GainExp(double ExpGained)
	{
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Skills] && playerData.instance.PlayerLevel < 76)
		{
			playerData.instance.PlayerExp += ExpGained;
			while (playerData.instance.PlayerExp >= DatabaseManager.ExpToLevelUpEquation(playerData.instance.PlayerLevel + 1))
			{
				playerData.instance.PlayerExp -= DatabaseManager.ExpToLevelUpEquation(playerData.instance.PlayerLevel + 1);
				playerData.instance.PlayerLevel++;
				ChangeCurrency(Currencies.LevelPoints, playerData.instance.stats.SkillCurrencyDrop.Total.RealValue);
				playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.LevelPoints] += playerData.instance.stats.SkillCurrencyDrop.Total.RealValue;
				playerData.instance.TotalCurrenciesGained_FullGame[Currencies.LevelPoints] += playerData.instance.stats.SkillCurrencyDrop.Total.RealValue;
				SkillsUIManager.instance.UpdateAllSkillUI();
			}
			UpdateLevelUI();
		}
	}

	public void GainPartialGoldOfALLNonDeadEnemies()
	{
		float num = EnemiesManager.instance.PartialEnemiesDead();
		double num2 = DatabaseManager.EnemyGold(playerData.instance.MonstersLevel);
		num2 *= (double)num;
		num2 *= (double)playerData.instance.stats.GoldGained.Total.RealValue;
		num2.ToReadable();
		if (RunManager.instance.isDoubleGoldRun && RunManager.instance.isDoubleLootRun)
		{
			num2 *= 4.0;
		}
		else if (RunManager.instance.isDoubleGoldRun || RunManager.instance.isDoubleLootRun)
		{
			num2 *= 2.0;
		}
		playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.Gold] += num2;
		playerData.instance.TotalCurrenciesGained_FullGame[Currencies.Gold] += num2;
		ChangeCurrency(Currencies.Gold, num2, isApplyGainMulti: false);
	}

	public void UpdateLevelUI()
	{
		LevelBar.ManageBar(playerData.instance.PlayerExp, DatabaseManager.ExpToLevelUpEquation(playerData.instance.PlayerLevel + 1));
		LevelText.text = playerData.instance.PlayerLevel.ToString();
		LevelBar_NonRun.ManageBar(playerData.instance.PlayerExp, DatabaseManager.ExpToLevelUpEquation(playerData.instance.PlayerLevel + 1));
		LevelText_NonRun.text = playerData.instance.PlayerLevel.ToString();
		if (playerData.instance.PlayerLevel >= 76)
		{
			LevelBar.BarText.text = LocalizerManager.GetTranslatedValue("Max_Text");
			LevelBar_NonRun.BarText.text = LocalizerManager.GetTranslatedValue("Max_Text");
		}
	}
}
public enum Currencies
{
	Gold = 1,
	LevelPoints,
	CharacterCurrency,
	GemCurrency,
	ClearCurrency,
	WellCurrency
}
public class ProjectileBehavior : MonoBehaviour
{
	private ProjectileSelfer AccPS;

	private List<int> alreadyTriggeredChainReactionEnemies = new List<int>();

	public void init()
	{
		AccPS = GetComponent<ProjectileSelfer>();
	}

	public void TriggerSkillOnEnemy(EnemySelfer TargetEnemy)
	{
		GenericHitEnemy(TargetEnemy);
		ProjectileInfo caller = AccPS.myInfo.Caller;
		if (AccPS.myInfo.functionName == "Spear_Projectile")
		{
			if (TargetEnemy.isDead || TargetEnemy == null)
			{
				EnemySelfer nearestEnemy = EnemiesManager.instance.GetNearestEnemy(base.transform.position, 150f);
				SkillsManager.instance.StartCoroutine(SkillsManager.instance.GenericChainSkill(caller, nearestEnemy));
			}
			else
			{
				SkillsManager.instance.StartCoroutine(SkillsManager.instance.GenericChainSkill(caller, TargetEnemy));
			}
			GenericHitEnemy(TargetEnemy);
		}
		else if (AccPS.myInfo.functionName == "Archer_Poison" || AccPS.myInfo.functionName == "Archer_Ice")
		{
			SkillsManager.instance.CallSkill(AccPS.myInfo, TargetEnemy, base.transform.position);
		}
	}

	public void OnHitAreaBehavior(Vector2 Position)
	{
		EnemySelfer nearestEnemy = EnemiesManager.instance.GetNearestEnemy(Position, 120f);
		if (nearestEnemy != null)
		{
			TriggerSkillOnEnemy(nearestEnemy);
		}
	}

	public void GenericHitEnemy(EnemySelfer TargetEnemy)
	{
		if (TargetEnemy == null || TargetEnemy.isDead || AccPS.myInfo.functionName == "ArcherToxin_Projectile" || AccPS.myInfo.functionName == "Archer_Poison")
		{
			return;
		}
		double multiplier = 1.0;
		if (AccPS.myInfo.functionName == "Archer_Poison" || AccPS.myInfo.functionName == "Archer_Ice" || AccPS.myInfo.functionName == "Archer_Normal" || AccPS.myInfo.functionName == "Archer_Explosive" || AccPS.myInfo.functionName == "IceShard_Projectile" || AccPS.myInfo.functionName == "ArcherToxin_Projectile")
		{
			multiplier = playerData.instance.stats.Archer_DamageMultiplier.Total.RealValue;
		}
		string moreinfo = "damage1";
		if (AccPS.myInfo.functionName == "VampireExpBat_Projectile")
		{
			moreinfo = "damage2";
			if (FunctionsNeeded.IsHappened(playerData.instance.stats.Vampire_ChanceForBloodDrain.Total.RealValue))
			{
				SkillsManager.instance.CallSkill(DatabaseManager.SkillDetailDict["VampireExplosion"].SpecialProjectile, EnemiesManager.instance.GetRandomEnemy(), Vector2.zero);
			}
		}
		else if (AccPS.myInfo.functionName == "RainOfArrows_Projectile" && FunctionsNeeded.IsHappened(playerData.instance.stats.RoA_ChanceForDragon.Total.RealValue))
		{
			SkillsManager.instance.CallSkill(DatabaseManager.SkillDetailDict["RainOfArrows"].SpecialProjectile, EnemiesManager.instance.GetRandomEnemy(), Vector2.zero);
		}
		if (AccPS.myInfo.functionName == "HitMultiEnemiesGroundClickable_Projectile")
		{
			TargetEnemy.TakeDamage(AccPS.HitMultiEnemiesGroundClickable_DamageData);
			return;
		}
		if (AccPS.myInfo.functionName == "IceShard_Projectile")
		{
			moreinfo = "damage2";
		}
		DamageData dataOfDamage = playerData.instance.stats.DamageCalculation(AccPS.myInfo.isTriggeredBySkill ? AccPS.myInfo.Caller.functionName : AccPS.myInfo.functionName, moreinfo, multiplier);
		if (AccPS.myInfo.functionName == "Mouse_Projectile" || AccPS.myInfo.functionName == "Mouse_Raged_Projectile")
		{
			dataOfDamage = playerData.instance.stats.DamageCalculation("mouse", "damage2");
		}
		if (AccPS.myInfo.functionName == "Archer_Explosive")
		{
			List<EnemySelfer> enemiesInCircle = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(base.transform.position, (float)AccPS.myInfo.Parameters["Radius"] * DatabaseManager.OneGameUnitToUnityUnit);
			for (int i = 0; i < enemiesInCircle.Count; i++)
			{
				enemiesInCircle[i].TakeDamage(dataOfDamage);
			}
		}
		else
		{
			TargetEnemy.TakeDamage(dataOfDamage);
		}
	}

	public void IceMain_HitEnemy(EnemySelfer TargetEnemy)
	{
	}

	private void IceShard_HitEnemy(EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			DamageData dataOfDamage = new DamageData(10.0, IsCrit: false, "");
			TargetEnemy.TakeDamage(dataOfDamage);
		}
	}
}
[CreateAssetMenu]
public class ProjectileInfo : SerializedScriptableObject
{
	[Header("General")]
	[HideInInspector]
	public string functionName;

	public bool isTriggeredBySkill;

	public ProjectileInfo Caller;

	public Sprite Icon_Large;

	public Sprite Icon_Small;

	public Color Icon_Color;

	public Sprite Icon_BG;

	public int BaseProjectileCount = 5;

	public List<SkillTags> tags;

	public Dictionary<string, double> Parameters;

	public bool isArcherProjectile;

	public bool IgnoreEffectsAmount;

	[Space(10f)]
	[Header("Projectile")]
	public float speed;

	public GameObject mainPrefab;

	public GameObject deathPrefab;

	public bool isSpawnDeathOnlyOnDeath;

	public GameObject FireSoundPrefab;

	public GameObject HitSoundPrefab;

	public float deafFXDuration = 1.5f;

	public float DestroyAfter = 1f;

	public float FullyDestroyWithTailAfter = 3f;

	public float angleIncrement = 20f;

	public ProjectileMotionBehaviour MotionBehaviour;

	[Header("Skill")]
	public bool isTheSkillAPorjectile;

	public ProjectileInfo triggeredProjectileInfo;

	public GameObject skill_mainPrefab;

	public GameObject skill_deathPrefab;

	public GameObject skill_chainEffectPrefab;

	public GameObject skill_FireSoundPrefab;

	public GameObject skill_HitSoundPrefab;

	public float skill_deafFXDuration = 1.5f;

	public float skill_DestroyAfter = 1f;

	public float BaseArea_ToDivideOn = 100f;
}
public enum SkillTags
{
	Call,
	Fall,
	Single,
	Area,
	Duration,
	Chain
}
public class ProjectileMover : MonoBehaviour
{
	private bool isInit;

	private Transform myTrans;

	private float TimeToCollision;

	private float currentTimer;

	[HideInInspector]
	public Vector3 MyVelocity;

	private float MySpeed;

	[HideInInspector]
	public bool isMove;

	private static LayerMask EnemyWall_LayerMask = -10000;

	private static LayerMask Wall_LayerMask = -10000;

	private Collider2D ThingToCollideWith;

	[HideInInspector]
	public bool isReachedEndOfJourney;

	[HideInInspector]
	public Vector2 StartPosition;

	private ProjectileSelfer AccPS;

	private float arcHeight;

	private float motionProgress;

	private Vector2 TargetPosition;

	private float DistanceToTarget;

	private List<Vector2> AbovePositions = new List<Vector2>();

	[HideInInspector]
	public float AboveYPos = 600f;

	private float AboveBounceAdditionalYPos = 300f;

	private int AboveIndex;

	public Vector2 MyDirection;

	private Vector2 CircularCenter;

	private float CircularRadius;

	private float CircularAngle;

	private float CircularAngularSpeed;

	public void TakeInfo(Vector2 Direction, Vector2 StartPosition, Vector2 TargetPosition)
	{
		this.StartPosition = StartPosition;
		this.TargetPosition = TargetPosition;
		isMove = false;
		isReachedEndOfJourney = false;
		if (!isInit)
		{
			isInit = true;
			myTrans = base.transform;
			AccPS = GetComponent<ProjectileSelfer>();
		}
		if ((int)EnemyWall_LayerMask < -1000)
		{
			EnemyWall_LayerMask = (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Wall"));
			Wall_LayerMask = 1 << LayerMask.NameToLayer("Wall");
		}
		ThingToCollideWith = null;
		MySpeed = AccPS.myInfo.speed;
		if (AccPS.myInfo.functionName == "TowerCircle_Projectile" || AccPS.myInfo.functionName == "TowerAoE_Projectile")
		{
			MySpeed = AccPS.myInfo.speed * playerData.instance.stats.TowersAttackSpeedMultiplier.Total.RealValue;
		}
		if (AccPS.myInfo.MotionBehaviour == ProjectileMotionBehaviour.Arc)
		{
			ChangeDirection_Arc(StartPosition, TargetPosition);
		}
		else if (AccPS.myInfo.MotionBehaviour == ProjectileMotionBehaviour.Above)
		{
			AboveIndex = -1;
			AbovePositions = new List<Vector2> { StartPosition, TargetPosition };
			ChangeDirection_Above(isBounce: false);
		}
		else if (AccPS.myInfo.MotionBehaviour == ProjectileMotionBehaviour.Circular)
		{
			ChangeDirection_Circular(TargetPosition);
		}
		else
		{
			ChangeDirection_Normal(Direction);
		}
	}

	public void ChangeDirection_Arc(Vector2 startPosition)
	{
		Vector2 normalized = (TargetPosition - StartPosition).normalized;
		float num = Mathf.Clamp(0.4f * DistanceToTarget, 120f, 10000f);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(startPosition, normalized, 3000f, Wall_LayerMask);
		if (raycastHit2D.collider != null)
		{
			float num2 = Vector2.Distance(raycastHit2D.point, startPosition);
			if (num2 <= 15f)
			{
				normalized *= -1f;
			}
			else if (num > num2)
			{
				num = num2 - 12f;
			}
		}
		Vector2 targetPosition = startPosition + normalized * num;
		MySpeed = Mathf.Clamp(MySpeed * 0.4f, 0.27f * AccPS.myInfo.speed, 10000f);
		ChangeDirection_Arc(startPosition, targetPosition);
	}

	public void ChangeDirection_Arc(Vector2 startPosition, Vector2 targetPosition)
	{
		StartPosition = startPosition;
		TargetPosition = targetPosition + UnityEngine.Random.insideUnitCircle * 10f;
		DistanceToTarget = Vector2.Distance(StartPosition, TargetPosition);
		motionProgress = 0f;
		arcHeight = DistanceToTarget * 0.285f;
		isMove = true;
	}

	public void ChangeDirection_Above(bool isBounce)
	{
		AboveIndex++;
		if (isBounce)
		{
			MySpeed = AccPS.myInfo.speed * 0.2f;
			DistanceToTarget = AboveBounceAdditionalYPos;
			AbovePositions.Add(AbovePositions[AbovePositions.Count - 1] + UnityEngine.Random.insideUnitCircle * 50f);
		}
		else
		{
			DistanceToTarget = Vector2.Distance(AbovePositions[AboveIndex], AbovePositions[AboveIndex + 1]);
			Vector2 normalized = (AbovePositions[AboveIndex + 1] - AbovePositions[AboveIndex]).normalized;
			Quaternion rotation = Quaternion.Euler(0f, 0f, FunctionsNeeded.CalculateAngle(normalized) + 90f);
			myTrans.rotation = rotation;
		}
		motionProgress = 0f;
		isMove = true;
	}

	public void ChangeDirection_Circular(Vector2 centerPosition)
	{
		CircularCenter = centerPosition;
		Vector2 vector = (Vector2)myTrans.position - CircularCenter;
		CircularRadius = vector.magnitude;
		CircularAngle = Mathf.Atan2(vector.y, vector.x);
		CircularAngularSpeed = MySpeed / CircularRadius;
		isMove = true;
	}

	public void ChangeDirection_Normal(Vector2 Direction, bool isSpecialNinjaGoingBackwards = false)
	{
		MyDirection = Direction;
		MyVelocity = new Vector3(Direction.x, Direction.y, 0f) * AccPS.myInfo.speed;
		if (isSpecialNinjaGoingBackwards)
		{
			StartCoroutine(ChangeRotationForSpecialNinja(Direction));
		}
		else
		{
			Quaternion rotation = Quaternion.Euler(0f, 0f, FunctionsNeeded.CalculateAngle(Direction) + 90f);
			myTrans.rotation = rotation;
		}
		ShootRaycast();
		isMove = true;
	}

	private IEnumerator ChangeRotationForSpecialNinja(Vector2 Direction)
	{
		AccPS.myCollider.enabled = false;
		yield return new WaitForSeconds(0.01f);
		AccPS.myCollider.enabled = true;
	}

	private void ShootRaycast()
	{
		RaycastHit2D[] array = Physics2D.RaycastAll(base.transform.position, MyVelocity, 3000f, EnemyWall_LayerMask);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit2D raycastHit2D = array[i];
			if (raycastHit2D.collider != null && raycastHit2D.collider != ThingToCollideWith)
			{
				ThingToCollideWith = raycastHit2D.collider;
				TimeToCollision = Vector2.Distance(raycastHit2D.point, base.transform.position) / AccPS.myInfo.speed;
				currentTimer = 0f;
				return;
			}
		}
		AccPS.DestroyBullet(isImmediatly: true, isSpawnDeathGFX: false);
	}

	private void FixedUpdate()
	{
		if (AccPS.isDead || !isMove)
		{
			return;
		}
		if (AccPS.myInfo.MotionBehaviour == ProjectileMotionBehaviour.Arc)
		{
			motionProgress += Time.fixedDeltaTime / (DistanceToTarget / MySpeed);
			Vector2 vector = Vector2.Lerp(StartPosition, TargetPosition, motionProgress);
			float num = arcHeight * (4f * motionProgress * (1f - motionProgress));
			vector.y += num;
			myTrans.position = vector;
			if (motionProgress < 1f)
			{
				Vector2 vector2 = Vector2.Lerp(StartPosition, TargetPosition, motionProgress + 0.01f);
				vector2.y += arcHeight * (4f * (motionProgress + 0.01f) * (1f - (motionProgress + 0.01f)));
				float z = FunctionsNeeded.CalculateAngle((vector2 - (Vector2)myTrans.position).normalized) + 90f;
				myTrans.rotation = Quaternion.Euler(0f, 0f, z);
			}
			if (motionProgress >= 1f)
			{
				AccPS.TriggerEffect_Arc_ReachedArea(TargetPosition);
			}
		}
		else if (AccPS.myInfo.MotionBehaviour == ProjectileMotionBehaviour.Above)
		{
			motionProgress += Time.fixedDeltaTime / (DistanceToTarget / MySpeed);
			if (AboveIndex >= 3)
			{
				Vector2 vector3 = Vector2.Lerp(AbovePositions[AbovePositions.Count - 2], AbovePositions[AbovePositions.Count - 1], motionProgress);
				float num2 = AboveBounceAdditionalYPos * (4f * motionProgress * (1f - motionProgress));
				vector3.y += num2;
				float z2 = FunctionsNeeded.CalculateAngle((vector3 - (Vector2)myTrans.position).normalized) + 90f;
				myTrans.rotation = Quaternion.Euler(0f, 0f, z2);
				myTrans.position = vector3;
			}
			else
			{
				Vector2 vector4 = Vector2.Lerp(AbovePositions[AboveIndex], AbovePositions[AboveIndex + 1], motionProgress);
				myTrans.position = vector4;
			}
			if (motionProgress >= 1f)
			{
				if (AboveIndex == AbovePositions.Count - 2 || AboveIndex >= 3)
				{
					AccPS.TriggerEffect_Above_ReachedArea(AbovePositions[AbovePositions.Count - 1]);
				}
				else
				{
					ChangeDirection_Above(isBounce: false);
				}
			}
		}
		else if (AccPS.myInfo.MotionBehaviour == ProjectileMotionBehaviour.Circular)
		{
			CircularAngle += CircularAngularSpeed * Time.fixedDeltaTime;
			Vector2 vector5 = CircularCenter + new Vector2(Mathf.Cos(CircularAngle) * CircularRadius, Mathf.Sin(CircularAngle) * CircularRadius);
			myTrans.position = vector5;
			float z3 = FunctionsNeeded.CalculateAngle(new Vector2(0f - Mathf.Sin(CircularAngle), Mathf.Cos(CircularAngle))) + 90f;
			myTrans.rotation = Quaternion.Euler(0f, 0f, z3);
		}
		else
		{
			myTrans.position += MyVelocity * Time.fixedDeltaTime;
			currentTimer += Time.fixedDeltaTime;
			if (currentTimer >= TimeToCollision)
			{
				AccPS.TriggerEffect_ReachedTarget(ThingToCollideWith);
			}
		}
		if (isReachedEndOfJourney && Vector2.Distance(base.transform.position, StartPosition) < 10f)
		{
			isReachedEndOfJourney = false;
			AccPS.DestroyBullet(isImmediatly: false, isSpawnDeathGFX: true);
		}
	}
}
public class ProjectileSelfer : MonoBehaviour
{
	public ProjectileInfo myInfo;

	private bool isInit;

	[HideInInspector]
	public Transform myTrans;

	private ProjectileBehavior AccPB;

	[HideInInspector]
	public ProjectileMover AccPM;

	private GameObject GFX_Main;

	private GameObject GFX_Tail;

	[HideInInspector]
	public Collider2D myCollider;

	[HideInInspector]
	public bool isDead;

	[HideInInspector]
	public List<int> EnemiesPierced = new List<int>();

	[HideInInspector]
	public List<int> EnemiesChained = new List<int>();

	private int RemainingChain;

	private int RemainingPierce;

	private int RemainingBounce;

	private List<int> AlreadyHitEnemies = new List<int>();

	private TrailRenderer TrailRen;

	[HideInInspector]
	public static ProjectileInfo IceShardInfo;

	[HideInInspector]
	public int NumberOfEnemiesHit;

	[HideInInspector]
	public int NumberOfWallHit;

	[HideInInspector]
	public float timerToDestroyMyself;

	[HideInInspector]
	public float LifeTime;

	[HideInInspector]
	public bool isAppliedHalfHealthBonus;

	[HideInInspector]
	public bool IsFiredFromTowerDirectly;

	private bool isCountNormalHits = true;

	private bool isMainProjectile;

	[HideInInspector]
	public DamageData HitMultiEnemiesGroundClickable_DamageData;

	private float ToxinRadius;

	private float ToxinDuration;

	private float ToxinTickTime;

	private float ToxinCurrentTimer;

	private DamageData ToxinDamageData;

	public bool isShowGFX = true;

	[HideInInspector]
	public void TakeInfo(Vector2 Direction, Vector2 StartPosition, Vector2 TargetPosition, bool IsFiredFromTowerDirectly)
	{
		this.IsFiredFromTowerDirectly = IsFiredFromTowerDirectly;
		timerToDestroyMyself = 0f;
		isCountNormalHits = true;
		isMainProjectile = !myInfo.isTriggeredBySkill;
		AlreadyHitEnemies = new List<int>();
		NumberOfEnemiesHit = 0;
		NumberOfWallHit = 0;
		EnemiesPierced = new List<int>();
		EnemiesChained = new List<int>();
		isDead = false;
		if (!isInit)
		{
			isInit = true;
			AccPB = GetComponent<ProjectileBehavior>();
			AccPB.init();
			AccPM = GetComponent<ProjectileMover>();
			myTrans = base.transform;
			GFX_Main = base.transform.Find("GFX_Main").gameObject;
			GFX_Tail = base.transform.Find("GFX_Tail").gameObject;
			foreach (Transform item in GFX_Tail.transform)
			{
				if (item.TryGetComponent<TrailRenderer>(out var component))
				{
					TrailRen = component;
					break;
				}
			}
			if (TryGetComponent<Collider2D>(out var component2))
			{
				myCollider = component2;
			}
		}
		if (myCollider != null)
		{
			myCollider.enabled = true;
		}
		GFX_Main.SetActive(value: true);
		GFX_Tail.SetActive(value: true);
		isShowGFX = true;
		if (!myInfo.IgnoreEffectsAmount && FunctionsNeeded.IsHappened(100f - playerData.instance.EffectsAmount))
		{
			GFX_Main.SetActive(value: false);
			GFX_Tail.SetActive(value: false);
			isShowGFX = false;
		}
		if (TrailRen != null)
		{
			TrailRen.Clear();
		}
		LifeTime = myInfo.DestroyAfter;
		if (myInfo.functionName == "TowerAoE_Projectile")
		{
			LifeTime = myInfo.DestroyAfter * playerData.instance.stats.TowersAttackSpeedMultiplier.Total.RealValue;
		}
		if (myInfo.isTriggeredBySkill)
		{
			if (myInfo.Caller.tags.Contains(SkillTags.Duration))
			{
				LifeTime = playerData.instance.stats.GetTime(myInfo.Caller.functionName);
			}
			if (myInfo.Caller.tags.Contains(SkillTags.Area))
			{
				float radiusOfEffectArea = playerData.instance.stats.GetRadiusOfEffectArea(myInfo.Caller.functionName);
				base.transform.localScale = Vector3.one * radiusOfEffectArea * DatabaseManager.OneGameUnitToUnityUnit / myInfo.Caller.BaseArea_ToDivideOn;
				isCountNormalHits = false;
			}
		}
		if (myInfo.functionName == "ArcherToxin_Projectile")
		{
			ToxinRadius = (float)myInfo.Caller.Parameters["Radius"];
			ToxinDuration = (float)myInfo.Caller.Parameters["Duration"];
			LifeTime = ToxinDuration;
			ToxinTickTime = (float)myInfo.Caller.Parameters["TickTime"];
			ToxinCurrentTimer = 0f;
			base.transform.localScale = Vector3.one * ToxinRadius * DatabaseManager.OneGameUnitToUnityUnit / myInfo.Caller.BaseArea_ToDivideOn;
			ParticleSystem component3 = GFX_Main.transform.Find("Special_Toxin").GetComponent<ParticleSystem>();
			ParticleSystem component4 = GFX_Main.transform.Find("Skulls").GetComponent<ParticleSystem>();
			component3.Stop();
			component4.Stop();
			ParticleSystem.MainModule main = component3.main;
			main.duration = ToxinDuration - 1f;
			ParticleSystem.MainModule main2 = component4.main;
			main2.duration = ToxinDuration - 1f;
			component3.Play();
			component4.Play();
			ToxinDamageData = playerData.instance.stats.DamageCalculation(myInfo.Caller.functionName, "damage1");
		}
		AccPM.TakeInfo(Direction, StartPosition, TargetPosition);
	}

	public void SetBPCM(int bounce = -1, int pierce = -1, int chain = -1)
	{
		RemainingChain = 0;
		RemainingPierce = 0;
		RemainingBounce = 0;
		if (myInfo.functionName == "ArcherDragon_Projectile" || myInfo.functionName == "ArcherToxin_Projectile")
		{
			RemainingBounce = 1000;
			RemainingPierce = 1000;
		}
		if (myInfo.isArcherProjectile)
		{
			RemainingChain = FunctionsNeeded.IsHappened_Over100Things(playerData.instance.stats.Archer_Chain.Total.RealValue);
			RemainingPierce = FunctionsNeeded.IsHappened_Over100Things(playerData.instance.stats.Archer_Pierce.Total.RealValue);
			RemainingBounce = FunctionsNeeded.IsHappened_Over100Things(playerData.instance.stats.Archer_Bounce.Total.RealValue);
		}
		if (myInfo.functionName == "Mouse_Projectile")
		{
			RemainingChain = playerData.instance.stats.MouseProjectile_Chain.Total.RealValue;
			RemainingPierce = playerData.instance.stats.MouseProjectile_Pierce.Total.RealValue;
			RemainingBounce = playerData.instance.stats.MouseProjectile_Bounce.Total.RealValue;
		}
		if (myInfo.functionName == "Mouse_Raged_Projectile")
		{
			RemainingBounce = playerData.instance.stats.MouseProjectile_Bounce.Total.RealValue;
			RemainingPierce = 1000;
		}
		if (myInfo.functionName == "TowerPierce_Projectile")
		{
			isCountNormalHits = false;
			RemainingPierce = 1000;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Enemy")
		{
			EnemySelfer component = collision.GetComponent<EnemySelfer>();
			AccPB.TriggerSkillOnEnemy(component);
			if (myInfo.functionName == "TowerCircle_Projectile")
			{
				SpawnDeathGFX(isDeath: false);
			}
		}
	}

	public int GetEnemiesInColliderArea()
	{
		if (myCollider == null)
		{
			return 0;
		}
		Collider2D[] array = new Collider2D[50];
		ContactFilter2D contactFilter = default(ContactFilter2D);
		contactFilter.useTriggers = true;
		int num = myCollider.Overlap(contactFilter, array);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			if (array[i].CompareTag("Enemy"))
			{
				num2++;
			}
		}
		return num2;
	}

	public void TriggerEffect_ReachedTarget(Collider2D collision)
	{
		if (isDead)
		{
			return;
		}
		if (collision.tag == "Enemy")
		{
			EnemySelfer component = collision.GetComponent<EnemySelfer>();
			if (component.isDead)
			{
				AccPM.ChangeDirection_Normal(AccPM.MyVelocity.normalized);
				return;
			}
			AlreadyHitEnemies.Add(component.EnemyHashcode);
			SpawnDeathGFX(isDeath: false);
			NumberOfEnemiesHit++;
			if (isCountNormalHits)
			{
				AccPB.TriggerSkillOnEnemy(component);
			}
			RemainingChain--;
			if (RemainingChain < 0)
			{
				RemainingPierce--;
				if (RemainingPierce < 0)
				{
					AccPM.isReachedEndOfJourney = true;
					DestroyBullet(isImmediatly: false, isSpawnDeathGFX: false);
				}
				else
				{
					AccPM.ChangeDirection_Normal(AccPM.MyVelocity.normalized);
				}
				return;
			}
			EnemySelfer nearestEnemy = EnemiesManager.instance.GetNearestEnemy(base.transform.position, AlreadyHitEnemies);
			if (nearestEnemy == null || nearestEnemy.isDead)
			{
				RemainingChain = 0;
				return;
			}
			Vector2 vector = base.transform.position;
			float f = FunctionsNeeded.CalculateAngle(nearestEnemy.GetPosition() - vector, IsRadian: true);
			Vector2 direction = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			AccPM.ChangeDirection_Normal(direction);
		}
		else if (collision.tag == "Wall")
		{
			NumberOfWallHit++;
			RemainingBounce--;
			if (RemainingBounce < 0)
			{
				AccPM.isReachedEndOfJourney = true;
				DestroyBullet(isImmediatly: false, isSpawnDeathGFX: true);
			}
			else
			{
				WallBounce(collision.name);
			}
		}
	}

	public void TriggerEffect_Arc_ReachedArea(Vector2 Pos)
	{
		if (!isDead)
		{
			SpawnDeathGFX(isDeath: false);
			NumberOfEnemiesHit++;
			AccPB.OnHitAreaBehavior(Pos);
			RemainingBounce--;
			if (RemainingBounce < 0)
			{
				AccPM.isReachedEndOfJourney = true;
				DestroyBullet(isImmediatly: false, isSpawnDeathGFX: true);
			}
			else
			{
				AccPM.ChangeDirection_Arc(base.transform.position);
			}
		}
	}

	public void TriggerEffect_Above_ReachedArea(Vector2 Pos)
	{
		if (!isDead)
		{
			SpawnDeathGFX(isDeath: false);
			NumberOfEnemiesHit++;
			AccPB.OnHitAreaBehavior(Pos);
			RemainingBounce--;
			if (RemainingBounce < 0)
			{
				AccPM.isReachedEndOfJourney = true;
				DestroyBullet(isImmediatly: false, isSpawnDeathGFX: true);
			}
			else
			{
				AccPM.ChangeDirection_Above(isBounce: true);
			}
		}
	}

	private void WallBounce(string WallName)
	{
		Vector2 inDirection = AccPM.MyVelocity.normalized;
		Vector2 inNormal = new Vector2(0f, 1f);
		switch (WallName)
		{
		case "TopWall":
			inNormal = new Vector2(0f, -1f);
			break;
		case "LeftWall":
			inNormal = new Vector2(1f, 0f);
			break;
		case "RightWall":
			inNormal = new Vector2(-1f, 0f);
			break;
		}
		AccPM.ChangeDirection_Normal(Vector2.Reflect(inDirection, inNormal));
	}

	public void DestroyBullet(bool isImmediatly, bool isSpawnDeathGFX, bool isDestroy = false)
	{
		if (!isDead || isImmediatly)
		{
			AccPM.isMove = false;
			if (isSpawnDeathGFX)
			{
				SpawnDeathGFX(isDeath: true);
			}
			isDead = true;
			if (isImmediatly)
			{
				CancelInvoke();
				GFX_Main.SetActive(value: false);
				GFX_Tail.SetActive(value: false);
				ObjectPooler.instance.ReturnObjectToPool(base.gameObject, myInfo.functionName + "main", isDestroy);
			}
			else
			{
				StartCoroutine(FullyDestroyWithTail());
			}
		}
	}

	private IEnumerator FullyDestroyWithTail()
	{
		AccPM.isMove = false;
		GFX_Main.SetActive(value: false);
		yield return new WaitForSeconds(myInfo.FullyDestroyWithTailAfter);
		if (TrailRen != null)
		{
			TrailRen.Clear();
		}
		GFX_Tail.SetActive(value: false);
		CancelInvoke();
		ObjectPooler.instance.ReturnObjectToPool(base.gameObject, myInfo.functionName + "main");
	}

	private void SpawnDeathGFX(bool isDeath)
	{
		if ((isDeath || !myInfo.isSpawnDeathOnlyOnDeath) && myInfo.deathPrefab != null)
		{
			Vector3 scale = Vector3.one * -1f;
			if (myInfo.functionName == "Archer_Explosive")
			{
				scale = Vector3.one * (float)myInfo.Parameters["Radius"] * DatabaseManager.OneGameUnitToUnityUnit * 1.1f;
			}
			Vector3 position = base.transform.position;
			position.z = 100f;
			if (isShowGFX)
			{
				FXManager.instance.SpawnGFX(myInfo.functionName + "death", position, myInfo.deafFXDuration, scale, (myInfo.MotionBehaviour == ProjectileMotionBehaviour.Above) ? 0f : (base.transform.rotation.eulerAngles.z - 90f), ForceShow: true);
				FXManager.instance.PlaySound(myInfo.functionName + "_HitSound", ForcePlay: true);
			}
		}
	}

	public void SpawnAlchemistDeathGFX(Vector3 position)
	{
	}

	private void DestroyAfterTime_UsedForSafety()
	{
		DestroyBullet(isImmediatly: true, isSpawnDeathGFX: true);
	}

	private void Update()
	{
		if (isDead)
		{
			return;
		}
		timerToDestroyMyself += Time.deltaTime;
		if (timerToDestroyMyself >= LifeTime)
		{
			if (!isMainProjectile && myInfo.Caller.tags.Contains(SkillTags.Duration) && FunctionsNeeded.IsHappened(playerData.instance.stats.GetChanceToResetAfterFinishing(myInfo.Caller.functionName)))
			{
				LifeTime += playerData.instance.stats.GetTime(myInfo.Caller.functionName);
				return;
			}
			DestroyAfterTime_UsedForSafety();
		}
		if (!(myInfo.functionName == "ArcherToxin_Projectile"))
		{
			return;
		}
		ToxinCurrentTimer += Time.deltaTime;
		if (ToxinCurrentTimer >= ToxinTickTime)
		{
			ToxinCurrentTimer = 0f;
			List<EnemySelfer> enemiesInCircle = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(base.transform.position, ToxinRadius * DatabaseManager.OneGameUnitToUnityUnit);
			for (int i = 0; i < enemiesInCircle.Count; i++)
			{
				enemiesInCircle[i].TakeDamage(ToxinDamageData);
			}
		}
	}
}
public enum ProjectileMotionBehaviour
{
	Normal,
	Arc,
	Above,
	Circular
}
public class ProjectilesManager : MonoBehaviour
{
	public static ProjectilesManager instance;

	public Transform ProjectilesParent;

	private Dictionary<int, ProjectileSelfer> AllProjectiles = new Dictionary<int, ProjectileSelfer>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}
		UnityEngine.Debug.Log("Destroyed Instance");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void FireProjectile(Vector2 StartPosition, Vector2 TargetPosition, ProjectileInfo ProjInfo, MultipleProjectileFormation formation, int howmany, float AdditionalPush_ForTwiceShot, bool isFiredFromTowerDirectly)
	{
		if (RunManager.instance.isRunStarted)
		{
			float f = FunctionsNeeded.CalculateAngle(TargetPosition - StartPosition, IsRadian: true);
			Vector2 direction = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			FireProjectile(StartPosition, TargetPosition, direction, ProjInfo, formation, howmany, AdditionalPush_ForTwiceShot, isFiredFromTowerDirectly);
		}
	}

	public void FireProjectile(Vector2 StartPosition, Vector2 TargetPosition, Vector2 Direction, ProjectileInfo ProjInfo, MultipleProjectileFormation formation, int howmany, float AdditionalPush_ForTwiceShot, bool isFiredFromTowerDirectly)
	{
		switch (formation)
		{
		case MultipleProjectileFormation.GMP:
		{
			float angleIncrement = ProjInfo.angleIncrement;
			float num3 = FunctionsNeeded.CalculateAngle(Direction);
			int num4 = ((UnityEngine.Random.Range(0, 2) == 0) ? 1 : (-1));
			float num5 = Mathf.Floor((float)howmany / 2f) * angleIncrement * (float)num4 + num3;
			for (int j = 0; j < howmany; j++)
			{
				Vector2 vector2 = new Vector2(Mathf.Cos(num5 * (MathF.PI / 180f)), Mathf.Sin(num5 * (MathF.PI / 180f)));
				Vector2 startPosition2 = StartPosition + vector2 * AdditionalPush_ForTwiceShot;
				FireSingleProjectile(startPosition2, TargetPosition, vector2, ProjInfo, isFiredFromTowerDirectly);
				num5 -= (float)num4 * angleIncrement;
			}
			break;
		}
		case MultipleProjectileFormation.Circular:
		{
			float num = FunctionsNeeded.CalculateAngle(Direction) + (float)UnityEngine.Random.Range(0, 360);
			float num2 = 360f / (float)howmany;
			for (int i = 0; i < howmany; i++)
			{
				Vector2 vector = new Vector2(Mathf.Cos(num * (MathF.PI / 180f)), Mathf.Sin(num * (MathF.PI / 180f)));
				Vector2 startPosition = StartPosition + vector * AdditionalPush_ForTwiceShot;
				FireSingleProjectile(startPosition, TargetPosition, vector, ProjInfo, isFiredFromTowerDirectly);
				num += num2;
			}
			break;
		}
		}
	}

	private void FireSingleProjectile(Vector2 StartPosition, Vector2 TargetPosition, Vector2 Direction, ProjectileInfo ProjInfo, bool isFiredFromTowerDirectly)
	{
		GameObject gameObject = ObjectPooler.instance.GiveMeObject(ProjInfo.functionName + "main", ProjectilesParent, StartPosition);
		int hashCode = gameObject.GetHashCode();
		if (!AllProjectiles.ContainsKey(hashCode))
		{
			AllProjectiles.Add(hashCode, gameObject.GetComponent<ProjectileSelfer>());
		}
		AllProjectiles[hashCode].TakeInfo(Direction, StartPosition, TargetPosition, isFiredFromTowerDirectly);
		AllProjectiles[hashCode].SetBPCM();
	}

	public void FireSingleHitMultiEnemiesGroundClickable(Vector2 StartPosition, Vector2 Direction, ProjectileInfo ProjInfo, DamageData dd)
	{
		GameObject gameObject = ObjectPooler.instance.GiveMeObject(ProjInfo.functionName + "main", ProjectilesParent, StartPosition);
		int hashCode = gameObject.GetHashCode();
		if (!AllProjectiles.ContainsKey(hashCode))
		{
			AllProjectiles.Add(hashCode, gameObject.GetComponent<ProjectileSelfer>());
		}
		AllProjectiles[hashCode].TakeInfo(Direction, StartPosition, StartPosition + Direction * 200f, IsFiredFromTowerDirectly: false);
		AllProjectiles[hashCode].HitMultiEnemiesGroundClickable_DamageData = dd;
	}

	public void DestroyAllProjectiles(bool isDestroy = false)
	{
		foreach (int item in AllProjectiles.Keys.ToList())
		{
			if (AllProjectiles[item].gameObject.activeInHierarchy)
			{
				AllProjectiles[item].CancelInvoke();
				AllProjectiles[item].StopAllCoroutines();
				AllProjectiles[item].DestroyBullet(isImmediatly: true, isSpawnDeathGFX: false, isDestroy);
			}
		}
	}
}
public enum MultipleProjectileFormation
{
	Volley,
	GMP,
	Circular
}
public class SkillBarsManager : MonoBehaviour
{
	public static SkillBarsManager instance;

	public Transform SkillBarsParent;

	private Dictionary<string, Image> SkillIcons = new Dictionary<string, Image>();

	private Dictionary<string, GameObject> LockIconGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> LockBlackoutGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> SkillIconsGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, BarSelfer> SkillBars = new Dictionary<string, BarSelfer>();

	private Dictionary<string, GameObject> SkillBarImageGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> SkillBarTextGOs = new Dictionary<string, GameObject>();

	private Dictionary<string, float> SkillsTimers = new Dictionary<string, float>();

	private Dictionary<string, float> SpecialSkillsTimers = new Dictionary<string, float>();

	private Dictionary<string, ProjectileInfo> SkillsProjectiles = new Dictionary<string, ProjectileInfo>();

	private List<string> SkillNames = new List<string>();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		Dictionary<int, SkillDetailInfo> dictionary = new Dictionary<int, SkillDetailInfo>();
		for (int i = 0; i < DatabaseManager.SkillDetailList.Count; i++)
		{
			dictionary.Add(DatabaseManager.SkillDetailList[i].OrderOfAppearance, DatabaseManager.SkillDetailList[i]);
		}
		int num = 0;
		foreach (Transform item in SkillBarsParent)
		{
			SkillDetailInfo skillDetailInfo = dictionary[num];
			SkillIcons.Add(skillDetailInfo.functionName, item.Find("Icon").GetComponent<Image>());
			SkillIconsGOs.Add(skillDetailInfo.functionName, item.Find("Icon").gameObject);
			LockIconGOs.Add(skillDetailInfo.functionName, item.Find("LockIcon").gameObject);
			LockBlackoutGOs.Add(skillDetailInfo.functionName, item.Find("LockBlackout").gameObject);
			SkillBarImageGOs.Add(skillDetailInfo.functionName, item.Find("CooldownBar").gameObject);
			SkillBarTextGOs.Add(skillDetailInfo.functionName, item.Find("CooldownText").gameObject);
			SkillBars.Add(skillDetailInfo.functionName, new BarSelfer(item.Find("CooldownBar").GetComponent<Image>(), item.Find("CooldownText").GetComponent<TextMeshProUGUI>(), BarTextFormat.Current, "0.0", ""));
			num++;
		}
		foreach (SkillDetailInfo skillDetail in DatabaseManager.SkillDetailList)
		{
			SkillsTimers.Add(skillDetail.functionName, 0f);
			SkillsProjectiles.Add(skillDetail.functionName, skillDetail.Projectile);
			SkillNames.Add(skillDetail.functionName);
			SpecialSkillsTimers.Add(skillDetail.SpecialProjectile.functionName, 0f);
			UpdateSkillUI(skillDetail);
		}
	}

	public void StartRun()
	{
		foreach (SkillDetailInfo skillDetail in DatabaseManager.SkillDetailList)
		{
			SkillsTimers[skillDetail.functionName] = playerData.instance.stats.SkillsCooldown.Total.RealValue + UnityEngine.Random.Range(-0.4f, 0.4f);
			if (SkillsTimers[skillDetail.functionName] <= 0f)
			{
				SkillsTimers[skillDetail.functionName] = 0.4f;
			}
			SpecialSkillsTimers[skillDetail.functionName] = playerData.instance.stats.CallAllSpecialSkillsEveryXSeconds.Total.RealValue;
			UpdateSkillUI(skillDetail);
		}
	}

	public void EndRun()
	{
	}

	public void UpdateSkillUI(SkillDetailInfo skill)
	{
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Skills])
		{
			if (playerData.instance.SkillIsUnlocked[skill.functionName])
			{
				SkillIconsGOs[skill.functionName].SetActive(value: true);
				LockIconGOs[skill.functionName].SetActive(value: false);
				LockBlackoutGOs[skill.functionName].SetActive(value: false);
				SkillBarImageGOs[skill.functionName].SetActive(value: true);
				SkillBarTextGOs[skill.functionName].SetActive(value: true);
				SkillIcons[skill.functionName].sprite = DatabaseManager.SkillDetailDict[skill.functionName].icon;
				SkillIcons[skill.functionName].color = DatabaseManager.SkillDetailDict[skill.functionName].iconColor;
				FunctionsNeeded.ConstrainImageSize(SkillIcons[skill.functionName].rectTransform, SkillIcons[skill.functionName], 65f, 65f);
			}
			else
			{
				SkillIconsGOs[skill.functionName].SetActive(value: false);
				LockIconGOs[skill.functionName].SetActive(value: true);
				LockBlackoutGOs[skill.functionName].SetActive(value: true);
				SkillBarImageGOs[skill.functionName].SetActive(value: false);
				SkillBarTextGOs[skill.functionName].SetActive(value: false);
			}
		}
	}

	public void CallAllSkills(int howmanyTimes, Vector2 triggerPosition)
	{
		foreach (string skillName in SkillNames)
		{
			if (playerData.instance.SkillIsUnlocked[skillName])
			{
				SkillsManager.instance.StartCoroutine(SkillsManager.instance.CallSkill_MultipleTimes_Coroutine(SkillsProjectiles[skillName], EnemiesManager.instance.GetNearestEnemy(triggerPosition, 400f), triggerPosition, howmanyTimes));
			}
		}
	}

	public void CallARandomUnlockedSkill()
	{
		List<string> list = new List<string>();
		foreach (string skillName in SkillNames)
		{
			if (playerData.instance.SkillIsUnlocked[skillName])
			{
				list.Add(skillName);
			}
		}
		if (list.Count > 0)
		{
			CallSkill(list.GetOneRandom());
		}
	}

	private void CallSkill(string skillName)
	{
		int num = 1;
		if (FunctionsNeeded.IsHappened(playerData.instance.stats.SkillsChanceForAnotherTrigger.Total.RealValue))
		{
			num = 2;
		}
		for (int i = 0; i < num; i++)
		{
			EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
			SkillsManager.instance.CallSkill(SkillsProjectiles[skillName], randomEnemy, Vector2.zero);
		}
	}

	private void Update()
	{
		if (!RunManager.instance.isRunStarted || !playerData.instance.UnlockedSystems[UnlockableSystems.Skills])
		{
			return;
		}
		foreach (string skillName in SkillNames)
		{
			if (!playerData.instance.SkillIsUnlocked[skillName])
			{
				continue;
			}
			SkillsTimers[skillName] -= Time.deltaTime;
			SkillBars[skillName].ManageBar(SkillsTimers[skillName], playerData.instance.stats.SkillsCooldown.Total.RealValue);
			if (SkillsTimers[skillName] <= 0f)
			{
				CallSkill(skillName);
				SkillsTimers[skillName] = playerData.instance.stats.SkillsCooldown.Total.RealValue + UnityEngine.Random.Range(-0.4f, 0.4f);
				if (SkillsTimers[skillName] <= 0f)
				{
					SkillsTimers[skillName] = 0.4f;
				}
			}
			if (playerData.instance.SkillUpgradesLevels[skillName][3] > 0 && playerData.instance.stats.CallAllSpecialSkillsEveryXSeconds.Total.RealValue >= 1f)
			{
				SpecialSkillsTimers[skillName] -= Time.deltaTime;
				if (SpecialSkillsTimers[skillName] <= 0f)
				{
					SpecialSkillsTimers[skillName] = playerData.instance.stats.CallAllSpecialSkillsEveryXSeconds.Total.RealValue;
					EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
					SkillsManager.instance.CallSkill(DatabaseManager.SkillDetailDict[skillName].SpecialProjectile, randomEnemy, Vector2.zero);
				}
			}
		}
	}
}
[CreateAssetMenu]
public class SkillDetailInfo : ScriptableObject
{
	[HideInInspector]
	public string functionName;

	public Sprite icon;

	public Color iconColor;

	public ProjectileInfo Projectile;

	public ProjectileInfo SpecialProjectile;

	public int OrderOfAppearance;

	public List<StatValueCostEquations> StatValueCostEquations;

	public int UnlockCost;

	public int UnlockLevel;
}
[Serializable]
public class StatValueCostEquations
{
	public StatInfo stat;

	public string valueEquation;

	public string costEquation;

	public StatValueCostEquations(StatInfo stat, string valueEquation, string costEquation)
	{
		this.stat = stat;
		this.valueEquation = valueEquation;
		this.costEquation = costEquation;
	}
}
[CreateAssetMenu]
public class SkillInfo : ScriptableObject
{
	[HideInInspector]
	public string functionName;

	public List<SkillTags> tags;

	public bool isProjectileSkill;

	public ProjectileInfo projectileInfo;

	public GameObject mainPrefab;

	public GameObject deathPrefab;

	public GameObject FireSoundPrefab;

	public GameObject HitSoundPrefab;

	public float DamagePercentage;

	public float ActivateAfter;

	public float deafFXDuration = 1.5f;

	public float DestroyAfter = 1f;
}
public class SkillsManager : SerializedMonoBehaviour
{
	public static SkillsManager instance;

	private ProjectileInfo VampireExpBat_Projectile;

	private float AboveYPos = 600f;

	private Dictionary<string, int> NumberOfTimesCalled = new Dictionary<string, int>();

	public Dictionary<string, double> TotalDamagePerSource = new Dictionary<string, double>();

	private float waitTimeBetweenTriggers = 0.37f;

	private int callChainSFXEveryXChain = 2;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		VampireExpBat_Projectile = DatabaseManager.ProjectileDict["VampireExpBat_Projectile"];
	}

	public void StartRun()
	{
	}

	public void EndRun()
	{
	}

	private void Update()
	{
	}

	public IEnumerator CallSkill_MultipleTimes_Coroutine(ProjectileInfo projInfo, EnemySelfer enemy, Vector2 triggerPosition, int howmanyTriggers)
	{
		for (int i = 0; i < howmanyTriggers; i++)
		{
			CallSkill(projInfo, enemy, triggerPosition);
			if (howmanyTriggers > 1 && i < howmanyTriggers - 1)
			{
				yield return new WaitForSeconds(waitTimeBetweenTriggers);
			}
			if (!RunManager.instance.isRunStarted)
			{
				break;
			}
		}
	}

	public void CallSkill(ProjectileInfo projInfo, EnemySelfer enemy, Vector2 triggerPosition, bool allowForNearestEnemy = true)
	{
		if (!RunManager.instance.isRunStarted)
		{
			return;
		}
		if (!NumberOfTimesCalled.ContainsKey(projInfo.functionName))
		{
			NumberOfTimesCalled.Add(projInfo.functionName, 0);
		}
		NumberOfTimesCalled[projInfo.functionName]++;
		if (enemy == null || enemy.isDead)
		{
			if (allowForNearestEnemy)
			{
				enemy = EnemiesManager.instance.GetNearestEnemy(triggerPosition, 150f);
			}
			if (enemy == null || enemy.isDead)
			{
				return;
			}
		}
		if (projInfo.functionName == "Lightning1_Projectile_LChain")
		{
			StartCoroutine(GenericChainSkill(projInfo, enemy));
		}
		else if (projInfo.functionName == "Knight2_Projectile_Spear")
		{
			StartCoroutine(Spear(projInfo, enemy));
		}
		else if (projInfo.functionName == "Vampire2_Projectile_BloodSuck")
		{
			StartCoroutine(GenericChainSkill(projInfo, enemy));
		}
		else if (projInfo.functionName == "Lightning2_Projectile_LStrike")
		{
			StartCoroutine(GenericSingleSkill(projInfo, enemy));
		}
		else if (projInfo.functionName == "Knight1_Projectile_AreaSlash")
		{
			StartCoroutine(KnightAoESlash(projInfo, enemy));
		}
		else if (projInfo.functionName == "Vampire1_Projectile_VampireExp")
		{
			StartCoroutine(GenericSingleSkill(projInfo, enemy));
		}
		else if (projInfo.functionName == "Archer2_Projectile_Dragon")
		{
			StartCoroutine(Dragon(projInfo, enemy));
		}
		else if (projInfo.functionName == "Ninja1_Projectile_Shuriken")
		{
			StartCoroutine(Shuriken(projInfo, enemy));
		}
		else if (projInfo.functionName == "Archer1_Projectile_RoA")
		{
			StartCoroutine(RainOfArrows(projInfo, enemy));
		}
		else if (projInfo.functionName == "Fire1_Projectile_Meteor")
		{
			StartCoroutine(Meteor(projInfo, enemy));
		}
		else if (projInfo.functionName == "Archer_Poison")
		{
			StartCoroutine(Toxin(projInfo, enemy));
		}
		else if (projInfo.functionName == "Archer_Ice")
		{
			StartCoroutine(IceShard(projInfo, enemy));
		}
	}

	public IEnumerator GenericChainSkill(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			int num = playerData.instance.stats.GetChainCount(projInfo.functionName);
			if (projInfo.functionName == "Lightning1_Projectile_LChain" && FunctionsNeeded.IsHappened(playerData.instance.stats.Well_SkillsChanceForEmpoweredEffects.Total.RealValue))
			{
				num += DatabaseManager.NumberOfEffectsForEmpoweredSkills;
			}
			float chainDistance = DatabaseManager.ChainDistance;
			List<int> excludedEnemies = new List<int> { TargetEnemy.EnemyHashcode };
			FXManager.instance.PlaySound(projInfo.functionName + "FireSoundSkill");
			yield return StartCoroutine(ProcessChainBranch(isFirst: true, projInfo, TargetEnemy, num, chainDistance, excludedEnemies));
		}
	}

	private IEnumerator ProcessChainBranch(bool isFirst, ProjectileInfo projInfo, EnemySelfer startEnemy, int remainingChains, float chainDistance, List<int> excludedEnemies)
	{
		if (startEnemy == null || startEnemy.isDead || remainingChains <= 0)
		{
			yield break;
		}
		if (isFirst)
		{
			DamageData dataOfDamage = playerData.instance.stats.DamageCalculation(projInfo.functionName, "damage1");
			startEnemy.TakeDamage(dataOfDamage);
			if (projInfo.functionName == "Lightning1_Projectile_LChain" && FunctionsNeeded.IsHappened(playerData.instance.stats.LC_ChanceForLightningStrike.Total.RealValue))
			{
				CallSkill(DatabaseManager.SkillDetailDict["LightningChain"].SpecialProjectile, EnemiesManager.instance.GetRandomEnemy(), Vector2.zero);
			}
		}
		Vector2 pos = startEnemy.transform.position;
		List<EnemySelfer> chainedEnemies = EnemiesManager.instance.GetChainedEnemies(pos, remainingChains - 1, chainDistance * DatabaseManager.OneGameUnitToUnityUnit, new List<int>(excludedEnemies));
		chainedEnemies.Insert(0, startEnemy);
		excludedEnemies.AddRange(chainedEnemies.Select((EnemySelfer e) => e.EnemyHashcode));
		Vector3 pos2 = chainedEnemies[0].GetPosition(isRandomize: false, isGround: true);
		pos2.z = 100f;
		GameObject gameObject = FXManager.instance.SpawnGFX(projInfo.functionName + "mainSkill", pos2, 2f, Vector3.one);
		bool isShown = gameObject != null;
		for (int i = 1; i < chainedEnemies.Count; i++)
		{
			if (chainedEnemies[i] == null || chainedEnemies[i].isDead)
			{
				continue;
			}
			if (isShown)
			{
				GroundEffectsManager.instance.SpawnChainEffect(projInfo.functionName + "chainEffect", chainedEnemies[i - 1].GetPosition(), chainedEnemies[i].GetPosition(), 0.8f);
			}
			yield return new WaitForSeconds(0.05f);
			if (chainedEnemies[i] == null || chainedEnemies[i].isDead)
			{
				continue;
			}
			Vector3 pos3 = chainedEnemies[i].GetPosition(isRandomize: false, isGround: true);
			pos3.z = 100f;
			if (isShown)
			{
				FXManager.instance.SpawnGFX(projInfo.functionName + "mainSkill", pos3, 2f, Vector3.one, 0f, ForceShow: true);
			}
			DamageData dataOfDamage2 = playerData.instance.stats.DamageCalculation(projInfo.functionName, "damage1");
			chainedEnemies[i].TakeDamage(dataOfDamage2);
			excludedEnemies.Add(chainedEnemies[i].EnemyHashcode);
			if (projInfo.functionName == "Lightning1_Projectile_LChain" && FunctionsNeeded.IsHappened(playerData.instance.stats.LC_ChanceForLightningStrike.Total.RealValue))
			{
				CallSkill(DatabaseManager.SkillDetailDict["LightningChain"].SpecialProjectile, EnemiesManager.instance.GetRandomEnemy(), Vector2.zero);
			}
			if (i < chainedEnemies.Count - 1 && FunctionsNeeded.IsHappened(playerData.instance.stats.GetChanceToForkAfterEachChain(projInfo.functionName)))
			{
				int num = chainedEnemies.Count - i - 1;
				if (num > 0)
				{
					StartCoroutine(ProcessChainBranch(isFirst: false, projInfo, chainedEnemies[i], num, chainDistance, new List<int>(excludedEnemies)));
				}
			}
			if (i < chainedEnemies.Count - 1)
			{
				if ((i - 1) % callChainSFXEveryXChain == 0 && isShown)
				{
					FXManager.instance.PlaySound(projInfo.functionName + "FireSoundSkill", ForcePlay: true);
				}
				yield return new WaitForSeconds(0.05f);
			}
		}
		if (!(projInfo.functionName == "Vampire2_Projectile_BloodSuck"))
		{
			yield break;
		}
		float Duration = playerData.instance.stats.GetTime(projInfo.functionName);
		float applyEveryXSeconds = 0.5f;
		float timer = applyEveryXSeconds;
		float totalDuration = Duration;
		while (timer <= totalDuration)
		{
			yield return new WaitForSeconds(applyEveryXSeconds);
			bool flag = true;
			for (int j = 0; j < chainedEnemies.Count; j++)
			{
				if (!chainedEnemies[j].isDead && !(chainedEnemies[j] == null))
				{
					flag = false;
					DamageData dataOfDamage3 = playerData.instance.stats.DamageCalculation(projInfo.functionName, "damage1");
					chainedEnemies[j].TakeDamage(dataOfDamage3);
					Vector3 vector = chainedEnemies[j].GetPosition(isRandomize: false, isGround: true);
					vector.z = 100f;
				}
			}
			if (flag || !RunManager.instance.isRunStarted)
			{
				break;
			}
			timer += applyEveryXSeconds;
			for (int num2 = chainedEnemies.Count - 1; num2 >= 0; num2--)
			{
				if (chainedEnemies[num2].isDead || chainedEnemies[num2] == null)
				{
					chainedEnemies.RemoveAt(num2);
				}
			}
			if (timer >= totalDuration && FunctionsNeeded.IsHappened(playerData.instance.stats.BloodDrain_ChanceToResetAfterFinishing.Total.RealValue))
			{
				totalDuration += Duration;
			}
		}
	}

	private IEnumerator GenericSingleSkill(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			Vector3 Pos = TargetEnemy.GetPosition(isRandomize: false, isGround: true) + UnityEngine.Random.insideUnitCircle * 35f;
			Pos.z = 100f;
			float Radius = playerData.instance.stats.GetRadiusOfEffectArea(projInfo.functionName);
			if (FXManager.instance.SpawnGFX(projInfo.functionName + "mainSkill", Pos, 3.5f, Vector3.one) != null)
			{
				FXManager.instance.PlaySound(projInfo.functionName + "FireSoundSkill", ForcePlay: true);
			}
			yield return new WaitForSeconds(0.085f);
			List<EnemySelfer> enemiesInCircle = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(Pos, Radius * DatabaseManager.OneGameUnitToUnityUnit);
			if (enemiesInCircle.Count == 0)
			{
				enemiesInCircle.Add(TargetEnemy);
			}
			for (int i = 0; i < enemiesInCircle.Count; i++)
			{
				DamageData dataOfDamage = playerData.instance.stats.DamageCalculation(projInfo.functionName, "damage1");
				enemiesInCircle[i].TakeDamage(dataOfDamage);
			}
			if (projInfo.functionName == "Vampire1_Projectile_VampireExp")
			{
				StartCoroutine(VampireExp_CallProjectiles(projInfo, TargetEnemy.EnemyHashcode, Pos));
			}
		}
	}

	private IEnumerator KnightAoESlash(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (TargetEnemy == null || TargetEnemy.isDead)
		{
			yield break;
		}
		Vector3 Pos = TargetEnemy.GetPosition(isRandomize: false, isGround: true) + UnityEngine.Random.insideUnitCircle * 50f;
		Pos.z = 100f;
		float Radius = playerData.instance.stats.GetRadiusOfEffectArea(projInfo.functionName);
		Vector3 scale = Vector3.one * Radius * DatabaseManager.OneGameUnitToUnityUnit / 135f;
		if (FXManager.instance.SpawnGFX(projInfo.functionName + "mainSkill", Pos, 1.5f, scale) != null)
		{
			FXManager.instance.PlaySound(projInfo.functionName + "FireSoundSkill", ForcePlay: true);
		}
		yield return new WaitForSeconds(0.2f);
		List<EnemySelfer> nearbyEnemies = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(Pos, Radius * DatabaseManager.OneGameUnitToUnityUnit);
		int CallCount = playerData.instance.stats.GetCallCount(projInfo.functionName);
		if (projInfo.functionName == "Knight1_Projectile_AreaSlash" && FunctionsNeeded.IsHappened(playerData.instance.stats.Well_SkillsChanceForEmpoweredEffects.Total.RealValue))
		{
			CallCount += DatabaseManager.NumberOfEffectsForEmpoweredSkills;
		}
		for (int i = 0; i < CallCount; i++)
		{
			for (int j = 0; j < nearbyEnemies.Count; j++)
			{
				DamageData dataOfDamage = playerData.instance.stats.DamageCalculation(projInfo.functionName, "damage1");
				if (!(nearbyEnemies[j] == null) && !nearbyEnemies[j].isDead)
				{
					nearbyEnemies[j].TakeDamage(dataOfDamage);
				}
			}
			if (FunctionsNeeded.IsHappened(playerData.instance.stats.Knight_ChanceForSpear.Total.RealValue))
			{
				CallSkill(DatabaseManager.SkillDetailDict["KnightSlash"].SpecialProjectile, EnemiesManager.instance.GetRandomEnemy(), Vector2.zero);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	public IEnumerator RainOfArrows(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		int CallCount = playerData.instance.stats.GetCallCount(projInfo.functionName);
		if (projInfo.functionName == "Archer1_Projectile_RoA" && FunctionsNeeded.IsHappened(playerData.instance.stats.Well_SkillsChanceForEmpoweredEffects.Total.RealValue))
		{
			CallCount += DatabaseManager.NumberOfEffectsForEmpoweredSkills;
		}
		List<EnemySelfer> enemiesInCircle = EnemiesManager.instance.GetEnemiesInCircle<EnemySelfer>(TargetEnemy.GetPosition(isRandomize: false, isGround: true), 450f, CallCount);
		List<Vector2> Positions = new List<Vector2>();
		for (int j = 0; j < CallCount; j++)
		{
			if (enemiesInCircle.Count > j && enemiesInCircle[j] != null)
			{
				Vector2 position = enemiesInCircle[j].GetPosition(isRandomize: false, isGround: true);
				Positions.Add(position);
			}
		}
		bool flag = false;
		for (int k = 0; k < Positions.Count; k++)
		{
			if (Vector2.Distance(Positions[k], TargetEnemy.GetPosition(isRandomize: false, isGround: true)) < 50f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Positions.RemoveAt(0);
			Positions.Add(TargetEnemy.GetPosition(isRandomize: false, isGround: true));
		}
		bool isTwinFall = FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceForTwinFall(projInfo.functionName));
		for (int i = 0; i < CallCount; i++)
		{
			if (Positions.Count > i && RunManager.instance.isRunStarted)
			{
				Vector2 vector = Positions[i] + new Vector2(UnityEngine.Random.Range(-100, 100), AboveYPos);
				ProjectilesManager.instance.FireProjectile(vector, Positions[i], projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
				if (isTwinFall)
				{
					ProjectilesManager.instance.FireProjectile(vector + new Vector2(150f, 0f), Positions[i], projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
				}
				if (i < 5)
				{
					yield return new WaitForSeconds(0.1f);
				}
				else if (i < 10)
				{
					yield return new WaitForSeconds(0.05f);
				}
			}
		}
	}

	private IEnumerator VampireExp_CallProjectiles(ProjectileInfo projInfo, int ExcludeEnemy, Vector2 Pos)
	{
		yield return new WaitForSeconds(0.1f);
		int num = playerData.instance.stats.GetCallCount(projInfo.functionName);
		if (projInfo.functionName == "Vampire1_Projectile_VampireExp" && FunctionsNeeded.IsHappened(playerData.instance.stats.Well_SkillsChanceForEmpoweredEffects.Total.RealValue))
		{
			num += DatabaseManager.NumberOfEffectsForEmpoweredSkills;
		}
		List<EnemySelfer> randomEnemies = EnemiesManager.instance.GetRandomEnemies(num, ExcludeEnemy);
		for (int i = 0; i < randomEnemies.Count; i++)
		{
			ProjectilesManager.instance.FireProjectile(Pos, randomEnemies[i].GetPosition(), VampireExpBat_Projectile, MultipleProjectileFormation.GMP, 1, 70f, isFiredFromTowerDirectly: false);
		}
	}

	private IEnumerator Dragon(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
			if (randomEnemy.GetHashCode() == TargetEnemy.GetHashCode())
			{
				Vector2 randomDirection = FunctionsNeeded.GetRandomDirection();
				Vector2 targetPosition = TargetEnemy.GetPosition() + randomDirection * 10f;
				ProjectilesManager.instance.FireProjectile(TargetEnemy.GetPosition(), targetPosition, randomDirection, projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
			}
			else
			{
				ProjectilesManager.instance.FireProjectile(TargetEnemy.GetPosition(), randomEnemy.GetPosition(), projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
			}
		}
		yield break;
	}

	private IEnumerator Toxin(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
			if (randomEnemy.GetHashCode() == TargetEnemy.GetHashCode())
			{
				Vector2 randomDirection = FunctionsNeeded.GetRandomDirection();
				Vector2 targetPosition = TargetEnemy.GetPosition() + randomDirection * 10f;
				ProjectilesManager.instance.FireProjectile(TargetEnemy.GetPosition(), targetPosition, randomDirection, projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
			}
			else
			{
				ProjectilesManager.instance.FireProjectile(TargetEnemy.GetPosition(), randomEnemy.GetPosition(), projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
			}
		}
		yield break;
	}

	private IEnumerator IceShard(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			EnemySelfer randomEnemy = EnemiesManager.instance.GetRandomEnemy();
			if (randomEnemy.GetHashCode() == TargetEnemy.GetHashCode())
			{
				Vector2 randomDirection = FunctionsNeeded.GetRandomDirection();
				Vector2 targetPosition = TargetEnemy.GetPosition() + randomDirection * 10f;
				ProjectilesManager.instance.FireProjectile(TargetEnemy.GetPosition(), targetPosition, randomDirection, projInfo.triggeredProjectileInfo, MultipleProjectileFormation.Circular, (int)projInfo.Parameters["NumberOfShards"], 80f, isFiredFromTowerDirectly: false);
			}
			else
			{
				ProjectilesManager.instance.FireProjectile(TargetEnemy.GetPosition(), randomEnemy.GetPosition(), projInfo.triggeredProjectileInfo, MultipleProjectileFormation.Circular, (int)projInfo.Parameters["NumberOfShards"], 80f, isFiredFromTowerDirectly: false);
			}
		}
		yield break;
	}

	private IEnumerator Shuriken(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null))
		{
			_ = TargetEnemy.isDead;
		}
		yield break;
	}

	private IEnumerator Meteor(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			Vector2 startPosition = TargetEnemy.GetPosition(isRandomize: false, isGround: true) + new Vector2(UnityEngine.Random.Range(-300, 300), AboveYPos);
			ProjectilesManager.instance.FireProjectile(startPosition, TargetEnemy.GetPosition(isRandomize: false, isGround: true), projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
			yield return null;
		}
	}

	private IEnumerator Spear(ProjectileInfo projInfo, EnemySelfer TargetEnemy)
	{
		if (!(TargetEnemy == null) && !TargetEnemy.isDead)
		{
			Vector2 vector = TargetEnemy.GetPosition(isRandomize: false, isGround: true) + new Vector2(UnityEngine.Random.Range(-300, 300), AboveYPos);
			ProjectilesManager.instance.FireProjectile(vector, TargetEnemy.GetPosition(isRandomize: false, isGround: true), projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
			if (FunctionsNeeded.IsHappened(playerData.instance.stats.ChanceForTwinFall(projInfo.functionName)))
			{
				ProjectilesManager.instance.FireProjectile(vector + new Vector2(150f, 0f), TargetEnemy.GetPosition(isRandomize: false, isGround: true), projInfo.triggeredProjectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: false);
			}
			yield return null;
		}
	}
}
public class SkillsUIManager : MonoBehaviour
{
	public static SkillsUIManager instance;

	public Transform SkillDetailsParent;

	private Dictionary<string, Image> SkillIcons = new Dictionary<string, Image>();

	private Dictionary<string, Image> SkillIconsBorders = new Dictionary<string, Image>();

	private Dictionary<string, TextMeshProUGUI> SkillNames = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, TextMeshProUGUI> SkillUnlockCost = new Dictionary<string, TextMeshProUGUI>();

	private Dictionary<string, Button> SkillUnlockButtons = new Dictionary<string, Button>();

	private Dictionary<string, List<TextMeshProUGUI>> SkillUpgradesDescription = new Dictionary<string, List<TextMeshProUGUI>>();

	private Dictionary<string, List<TextMeshProUGUI>> SkillUpgradesCost = new Dictionary<string, List<TextMeshProUGUI>>();

	private Dictionary<string, List<Button>> SkillUpgradesButtons = new Dictionary<string, List<Button>>();

	private Dictionary<string, List<GameObject>> SkillUpgradesGOs = new Dictionary<string, List<GameObject>>();

	private Dictionary<string, GameObject> MaxOutForSpecialGO = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> UpgradeParentGOs = new Dictionary<string, GameObject>();

	private int MaxLevel = 3;

	private int MaxLevelForSpecial = 1;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.LevelPoints] = (Action)Delegate.Combine(onCurrencyChange[Currencies.LevelPoints], new Action(ManageAllPurchaseButtons));
		Dictionary<int, SkillDetailInfo> dictionary = new Dictionary<int, SkillDetailInfo>();
		for (int i = 0; i < DatabaseManager.SkillDetailList.Count; i++)
		{
			dictionary.Add(DatabaseManager.SkillDetailList[i].OrderOfAppearance, DatabaseManager.SkillDetailList[i]);
		}
		int num = 0;
		foreach (Transform item in SkillDetailsParent)
		{
			SkillDetailInfo skill = dictionary[num];
			SkillIcons.Add(skill.functionName, item.Find("Top").Find("SkillName").Find("IconPlace")
				.Find("Icon")
				.GetComponent<Image>());
			SkillIconsBorders.Add(skill.functionName, item.Find("Top").Find("SkillName").Find("IconPlace")
				.Find("Border")
				.GetComponent<Image>());
			SkillNames.Add(skill.functionName, item.Find("Top").Find("SkillName").GetComponent<TextMeshProUGUI>());
			SkillUnlockCost.Add(skill.functionName, item.Find("UnlockButton").Find("CostText").GetComponent<TextMeshProUGUI>());
			SkillUnlockButtons.Add(skill.functionName, item.Find("UnlockButton").GetComponent<Button>());
			SkillUnlockButtons[skill.functionName].onClick.AddListener(delegate
			{
				ClickedOnUnlockSkill(skill.functionName);
			});
			MaxOutForSpecialGO.Add(skill.functionName, item.Find("MaxOutForSpecial").gameObject);
			SkillUpgradesDescription.Add(skill.functionName, new List<TextMeshProUGUI>());
			SkillUpgradesCost.Add(skill.functionName, new List<TextMeshProUGUI>());
			SkillUpgradesButtons.Add(skill.functionName, new List<Button>());
			SkillUpgradesGOs.Add(skill.functionName, new List<GameObject>());
			Transform transform2 = item.Find("UpgradesParent");
			UpgradeParentGOs.Add(skill.functionName, transform2.gameObject);
			int num2 = 0;
			foreach (Transform item2 in transform2)
			{
				SkillUpgradesDescription[skill.functionName].Add(item2.Find("UpgradeText").GetComponent<TextMeshProUGUI>());
				SkillUpgradesCost[skill.functionName].Add(item2.Find("PurchaseButton").Find("CostText").GetComponent<TextMeshProUGUI>());
				SkillUpgradesButtons[skill.functionName].Add(item2.Find("PurchaseButton").GetComponent<Button>());
				SkillUpgradesGOs[skill.functionName].Add(item2.gameObject);
				int currentUpgIndex = num2;
				SkillUpgradesButtons[skill.functionName][num2].onClick.AddListener(delegate
				{
					ClickedOnUpgradeSkill(skill.functionName, currentUpgIndex);
				});
				num2++;
			}
			SkillUpgradesDescription[skill.functionName].Add(item.Find("SpecialUpgrade").Find("UpgradeText").GetComponent<TextMeshProUGUI>());
			SkillUpgradesCost[skill.functionName].Add(item.Find("SpecialUpgrade").Find("PurchaseButton").Find("CostText")
				.GetComponent<TextMeshProUGUI>());
			SkillUpgradesButtons[skill.functionName].Add(item.Find("SpecialUpgrade").Find("PurchaseButton").GetComponent<Button>());
			SkillUpgradesGOs[skill.functionName].Add(item.Find("SpecialUpgrade").gameObject);
			int currentUpgIndex2 = num2;
			SkillUpgradesButtons[skill.functionName][currentUpgIndex2].onClick.AddListener(delegate
			{
				ClickedOnUpgradeSkill(skill.functionName, currentUpgIndex2);
			});
			num++;
		}
		UpdateAllSkillUI();
	}

	public void UpdateAllSkillUI()
	{
		foreach (SkillDetailInfo skillDetail in DatabaseManager.SkillDetailList)
		{
			UpdateSkillUI(skillDetail);
		}
	}

	public void UpdateSkillUI(SkillDetailInfo skill)
	{
		SkillIcons[skill.functionName].sprite = DatabaseManager.SkillDetailDict[skill.functionName].icon;
		SkillIcons[skill.functionName].color = DatabaseManager.SkillDetailDict[skill.functionName].iconColor;
		SkillIconsBorders[skill.functionName].color = DatabaseManager.SkillDetailDict[skill.functionName].iconColor;
		SkillNames[skill.functionName].text = LocalizerManager.GetTranslatedValue(skill.functionName + "_Name");
		SkillNames[skill.functionName].color = DatabaseManager.SkillDetailDict[skill.functionName].iconColor;
		FunctionsNeeded.ConstrainImageSize(SkillIcons[skill.functionName].rectTransform, SkillIcons[skill.functionName], 45f, 45f);
		if (playerData.instance.SkillIsUnlocked[skill.functionName])
		{
			SkillUnlockButtons[skill.functionName].gameObject.SetActive(value: false);
			UpgradeParentGOs[skill.functionName].SetActive(value: true);
			bool flag = true;
			bool flag2 = true;
			for (int i = 0; i < skill.StatValueCostEquations.Count; i++)
			{
				if (i < skill.StatValueCostEquations.Count - 1)
				{
					if (flag)
					{
						SkillUpgradesGOs[skill.functionName][i].SetActive(value: true);
						int num = playerData.instance.SkillUpgradesLevels[skill.functionName][i];
						double item = ExpressionEvaluator.Evaluate(skill.StatValueCostEquations[i].valueEquation, (num <= 0) ? 1 : num);
						SkillUpgradesDescription[skill.functionName][i].text = skill.StatValueCostEquations[i].stat.GetValueDescText_SingleOrMultipleValues(new List<double> { item }, isColoredTag: false);
						flag = num > 0;
						if (num < MaxLevel)
						{
							flag2 = false;
							int num2 = Mathf.RoundToInt((float)ExpressionEvaluator.Evaluate(skill.StatValueCostEquations[i].costEquation, num + 1));
							SkillUpgradesCost[skill.functionName][i].text = "<sprite name=LevelPoints>" + num2;
						}
						else
						{
							SkillUpgradesCost[skill.functionName][i].text = LocalizerManager.GetTranslatedValue("Max_Text");
						}
					}
					else
					{
						flag2 = false;
						SkillUpgradesGOs[skill.functionName][i].SetActive(value: false);
					}
				}
				else if (flag2)
				{
					SkillUpgradesGOs[skill.functionName][i].SetActive(value: true);
					MaxOutForSpecialGO[skill.functionName].SetActive(value: false);
					int num3 = playerData.instance.SkillUpgradesLevels[skill.functionName][i];
					if (num3 < MaxLevelForSpecial)
					{
						int num4 = Mathf.RoundToInt((float)ExpressionEvaluator.Evaluate(skill.StatValueCostEquations[i].costEquation, num3 + 1));
						SkillUpgradesCost[skill.functionName][i].text = "<sprite name=LevelPoints>" + num4;
					}
					else
					{
						SkillUpgradesCost[skill.functionName][i].text = LocalizerManager.GetTranslatedValue("Max_Text");
					}
					double item2 = ExpressionEvaluator.Evaluate(skill.StatValueCostEquations[i].valueEquation, (num3 <= 0) ? 1 : num3);
					SkillUpgradesDescription[skill.functionName][i].text = skill.StatValueCostEquations[i].stat.GetValueDescText_SingleOrMultipleValues(new List<double> { item2 }, isColoredTag: false);
				}
				else if (flag)
				{
					SkillUpgradesGOs[skill.functionName][i].SetActive(value: false);
					MaxOutForSpecialGO[skill.functionName].SetActive(value: true);
				}
				else
				{
					SkillUpgradesGOs[skill.functionName][i].SetActive(value: false);
					MaxOutForSpecialGO[skill.functionName].SetActive(value: false);
				}
			}
		}
		else
		{
			SkillUnlockButtons[skill.functionName].gameObject.SetActive(value: true);
			UpgradeParentGOs[skill.functionName].SetActive(value: false);
			if (playerData.instance.PlayerLevel < skill.UnlockLevel)
			{
				SkillUnlockCost[skill.functionName].text = LocalizerManager.GetTranslatedThenReplaceValues("UnlockableLevel_Text", skill.UnlockLevel.ToString());
			}
			else
			{
				SkillUnlockCost[skill.functionName].text = LocalizerManager.GetTranslatedValue("Unlock_Text") + "  <sprite name=LevelPoints>" + skill.UnlockCost;
			}
		}
		ManageAllPurchaseButtons();
	}

	private void ManageAllPurchaseButtons()
	{
		foreach (SkillDetailInfo skillDetail in DatabaseManager.SkillDetailList)
		{
			if (playerData.instance.SkillIsUnlocked[skillDetail.functionName])
			{
				for (int i = 0; i < skillDetail.StatValueCostEquations.Count; i++)
				{
					int num = playerData.instance.SkillUpgradesLevels[skillDetail.functionName][i];
					if ((i < skillDetail.StatValueCostEquations.Count - 1 && num == MaxLevel) || (i == skillDetail.StatValueCostEquations.Count - 1 && num == MaxLevelForSpecial))
					{
						SkillUpgradesButtons[skillDetail.functionName][i].interactable = false;
						continue;
					}
					int num2 = Mathf.RoundToInt((float)ExpressionEvaluator.Evaluate(skillDetail.StatValueCostEquations[i].costEquation, num + 1));
					SkillUpgradesButtons[skillDetail.functionName][i].interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.LevelPoints, num2, IsSpendAlso: false);
				}
			}
			else if (playerData.instance.PlayerLevel < skillDetail.UnlockLevel)
			{
				SkillUnlockButtons[skillDetail.functionName].interactable = false;
			}
			else
			{
				SkillUnlockButtons[skillDetail.functionName].interactable = PlayerManager.instance.IsCanSpendCurrency(Currencies.LevelPoints, skillDetail.UnlockCost, IsSpendAlso: false);
			}
		}
	}

	public void CheckNotifications_EndRun()
	{
		foreach (SkillDetailInfo skillDetail in DatabaseManager.SkillDetailList)
		{
			if (playerData.instance.SkillIsUnlocked[skillDetail.functionName])
			{
				for (int i = 0; i < skillDetail.StatValueCostEquations.Count; i++)
				{
					int num = playerData.instance.SkillUpgradesLevels[skillDetail.functionName][i];
					if ((i >= skillDetail.StatValueCostEquations.Count - 1 || num != MaxLevel) && (i != skillDetail.StatValueCostEquations.Count - 1 || num != MaxLevelForSpecial))
					{
						int num2 = Mathf.RoundToInt((float)ExpressionEvaluator.Evaluate(skillDetail.StatValueCostEquations[i].costEquation, num + 1));
						if (PlayerManager.instance.IsCanSpendCurrency(Currencies.LevelPoints, num2, IsSpendAlso: false))
						{
							MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Skills, isShow: true);
							return;
						}
					}
				}
			}
			else if (PlayerManager.instance.IsCanSpendCurrency(Currencies.LevelPoints, skillDetail.UnlockCost, IsSpendAlso: false))
			{
				MainMenusManager.instance.ShowHideNotificationInSystem(UnlockableSystems.Skills, isShow: true);
				break;
			}
		}
	}

	public void ClickedOnUnlockSkill(string skillName)
	{
		SkillDetailInfo skillDetailInfo = DatabaseManager.SkillDetailDict[skillName];
		if (!playerData.instance.SkillIsUnlocked[skillName] && PlayerManager.instance.IsCanSpendCurrency(Currencies.LevelPoints, skillDetailInfo.UnlockCost, IsSpendAlso: true))
		{
			FXManager.instance.PlayUIClickSound();
			playerData.instance.SkillIsUnlocked[skillName] = true;
			switch (skillName)
			{
			case "LightningChain":
				AchievementsManager.instance.UnlockAchievement("SkillLC");
				break;
			case "RainOfArrows":
				AchievementsManager.instance.UnlockAchievement("SkillRoA");
				break;
			case "KnightSlash":
				AchievementsManager.instance.UnlockAchievement("SkillKnight");
				break;
			case "VampireExplosion":
				AchievementsManager.instance.UnlockAchievement("SkillVamp");
				break;
			}
			UpdateSkillUI(skillDetailInfo);
		}
	}

	public void ClickedOnUpgradeSkill(string skillName, int upgradeIndex)
	{
		SkillDetailInfo skillDetailInfo = DatabaseManager.SkillDetailDict[skillName];
		int num = playerData.instance.SkillUpgradesLevels[skillDetailInfo.functionName][upgradeIndex];
		int num2 = Mathf.RoundToInt((float)ExpressionEvaluator.Evaluate(skillDetailInfo.StatValueCostEquations[upgradeIndex].costEquation, num + 1));
		if (PlayerManager.instance.IsCanSpendCurrency(Currencies.LevelPoints, num2, IsSpendAlso: true))
		{
			FXManager.instance.PlayUIClickSound();
			playerData.instance.SkillUpgradesLevels[skillDetailInfo.functionName][upgradeIndex]++;
			ApplySkillUpgrade(skillDetailInfo, upgradeIndex);
			UpdateSkillUI(skillDetailInfo);
		}
	}

	private void ApplySkillUpgrade(SkillDetailInfo skill, int upgradeIndex)
	{
		int num = playerData.instance.SkillUpgradesLevels[skill.functionName][upgradeIndex];
		StatInfo stat = skill.StatValueCostEquations[upgradeIndex].stat;
		double num2 = ((num > 0) ? ExpressionEvaluator.Evaluate(skill.StatValueCostEquations[upgradeIndex].valueEquation, num - 1) : 0.0);
		double value = ExpressionEvaluator.Evaluate(skill.StatValueCostEquations[upgradeIndex].valueEquation, num) - num2;
		playerData.instance.stats.ChangeAStat(stat.VariableName, stat.StatsProp, value, IsAdd: true);
	}
}
public class EnemyStatsData : StatsData
{
	public StatsDouble Health = new StatsDouble();

	public StatsFloat DamageTaken = new StatsFloat(DatabaseManager.DamageTakenMinMax);

	private double _CurrentHealth;

	public double CurrentHealth
	{
		get
		{
			return FunctionsNeeded.Clamp(_CurrentHealth, 0.0, Health.Total.RealValue);
		}
		set
		{
			_CurrentHealth = value;
		}
	}

	public event Action OnStatsUpgrade;

	public event Action OnHealthChange;

	public void InitEnemyStats(bool isGhostBoss)
	{
		Init();
		if (isGhostBoss)
		{
			ChangeAStat("Health", StatsProperties.Flat, DatabaseManager.EnemyHealth(playerData.instance.MonstersLevel) * 2.0, IsAdd: true);
		}
		else
		{
			ChangeAStat("Health", StatsProperties.Flat, DatabaseManager.EnemyHealth(playerData.instance.MonstersLevel + (RunManager.instance.IsBossRun ? 1 : 0)), IsAdd: true);
		}
		CurrentHealth = Health.Total.RealValue;
	}

	public override void ChangeAStat(string StatFunctionName, StatsProperties SProp, double Value, bool IsAdd)
	{
		if (StatFunctionName == "Health")
		{
			ChangeAStat_Health(StatFunctionName, SProp, Value, IsAdd);
			return;
		}
		base.ChangeAStat(StatFunctionName, SProp, Value, IsAdd);
		this.OnStatsUpgrade?.Invoke();
	}

	private void ChangeAStat_Health(string StatFunctionName, StatsProperties SProp, double Value, bool IsAdd)
	{
		double value = 0.0;
		if (Health.Total.RealValue != 0.0)
		{
			value = CurrentHealth / Health.Total.RealValue;
		}
		double realValue = Health.Total.RealValue;
		double currentHealth = CurrentHealth;
		ChangeStatsDouble(Health, SProp, Value, IsAdd);
		switch (SProp)
		{
		case StatsProperties.Flat:
			CurrentHealth = currentHealth + (Health.Total.RealValue - realValue);
			break;
		case StatsProperties.Additive:
		case StatsProperties.Multiplicative:
			value = FunctionsNeeded.Clamp(value, 0.0, 1.0);
			CurrentHealth = value * Health.Total.RealValue;
			break;
		}
		this.OnHealthChange?.Invoke();
		this.OnStatsUpgrade?.Invoke();
	}
}
[Serializable]
public class PlayerStatsData : StatsData
{
	public StatsDouble Damage = new StatsDouble();

	public StatsFloat ChanceForTwiceHits = new StatsFloat();

	public StatsFloat ChanceForDoubleDamage = new StatsFloat();

	public StatsFloat ChanceForTripleDamage = new StatsFloat();

	public StatsFloat OverkillDamageMultiplier = new StatsFloat();

	public StatsFloat MouseRadius = new StatsFloat();

	public StatsFloat MouseAttackSpeed = new StatsFloat();

	public StatsFloat Timer = new StatsFloat();

	public StatsFloat TimerOnMonsterDeath = new StatsFloat();

	public StatsFloat ChanceToFireMouseProjectile = new StatsFloat();

	public StatsInt MouseProjectile_Bounce = new StatsInt();

	public StatsInt MouseProjectile_Pierce = new StatsInt();

	public StatsInt MouseProjectile_Chain = new StatsInt();

	public StatsInt MouseProjectile_AdditionalProjectiles = new StatsInt();

	public StatsFloat MouseProjectile_DamageMultiplier = new StatsFloat();

	public StatsFloat ChanceForMouseAttackToBeRaged = new StatsFloat();

	public StatsFloat ChanceForIdleMouseAttackToBeRaged = new StatsFloat();

	public StatsFloat RagedMouseAreaMultiplier = new StatsFloat();

	public StatsFloat RagedMouseDamageMultiplier = new StatsFloat();

	public StatsFloat RagedMouseChanceToFireRagedProjectile = new StatsFloat();

	public StatsFloat IdleMouseAttackCooldown = new StatsFloat();

	public StatsInt NumberOfIdleMouseAttacks = new StatsInt();

	public StatsInt TriggersOfIdleMouseAttacks = new StatsInt();

	public StatsFloat MouseAttack_DamageMultiplier = new StatsFloat();

	public StatsFloat GoldGained = new StatsFloat();

	public StatsFloat ExpGained = new StatsFloat();

	public StatsFloat ChanceToManyXGoldGained = new StatsFloat();

	public StatsFloat ItemsRarity = new StatsFloat();

	public StatsFloat ItemsChance = new StatsFloat();

	public StatsInt NumberOfMonsters = new StatsInt(new MinMax(0.0, 100.0));

	public StatsInt MonstersSpawnInClusters = new StatsInt();

	public StatsInt MonstersDropPartialGold = new StatsInt();

	public StatsInt NumberOfMonstersSpawned = new StatsInt();

	public StatsFloat NonNormalMonsterChance = new StatsFloat();

	public StatsFloat TreasureChance = new StatsFloat();

	public StatsFloat TreasureGoldMultiplier = new StatsFloat();

	public StatsFloat TreasureExpMultiplier = new StatsFloat();

	public StatsFloat MonstersSpawnRate = new StatsFloat();

	public StatsFloat ChanceToSpawnMonsterOnDeath = new StatsFloat(new MinMax(0.0, 90.0));

	public StatsFloat ShinyChance = new StatsFloat();

	public StatsFloat ShinyRarity = new StatsFloat();

	public StatsInt ShinyRare_CanBeFoundInRun = new StatsInt();

	public StatsInt ShinyEpic_CanBeFoundInRun = new StatsInt();

	public StatsFloat GhostChance = new StatsFloat();

	public StatsInt CharacterCurrencyDrop = new StatsInt();

	public StatsFloat OreChance = new StatsFloat();

	public StatsFloat OreChance_Rich = new StatsFloat();

	public StatsInt OreDrop_GemCurrency = new StatsInt();

	public StatsFloat RichOreMultiplier = new StatsFloat();

	public StatsFloat ChanceToSpawnBounty = new StatsFloat();

	public StatsFloat BountiesEffect = new StatsFloat();

	public StatsInt Bounty_SpawnShiny_CanBeFoundInRun = new StatsInt();

	public StatsInt Bounty_DropItem_CanBeFoundInRun = new StatsInt();

	public StatsInt Bounty_CallSkills_CanBeFoundInRun = new StatsInt();

	public StatsFloat DamageMultiplier_PerTotalShinyFound = new StatsFloat();

	public StatsFloat DamageMultiplier_PerStatsInAllItemsEquipped = new StatsFloat();

	public StatsFloat DamageMultiplier_PerBountyFound = new StatsFloat();

	public StatsFloat DamageMultiplier_PerLevelOfGems = new StatsFloat();

	public StatsFloat DamageMultiplier_PerSecondInTimer = new StatsFloat();

	public StatsFloat DamageMultiplier_PerAreaMarkApplied = new StatsFloat();

	public StatsFloat DamageMultiplier_PerMonster = new StatsFloat();

	public StatsFloat ChanceToSpawnDebuff = new StatsFloat();

	public StatsFloat DebuffRadius = new StatsFloat();

	public StatsInt Debuff_SpawnShiny_CanBeFoundInRun = new StatsInt();

	public StatsInt Debuff_CallSkills_CanBeFoundInRun = new StatsInt();

	public StatsInt Debuff_DamageTaken_CanBeFoundInRun = new StatsInt();

	public StatsInt Debuff_GoldDropped_CanBeFoundInRun = new StatsInt();

	public StatsFloat Debuff_GoldMultiplier = new StatsFloat();

	public StatsFloat Debuff_DamageMultiplier = new StatsFloat();

	public StatsFloat Debuff_SkillChance = new StatsFloat();

	public StatsFloat ChanceForDebuffToTargetDenseAreas = new StatsFloat();

	public StatsFloat ChanceOnAnyHitToApplyDebuff = new StatsFloat();

	public StatsFloat TimeGainedWheneverAMonsterIsDebuffed = new StatsFloat();

	public StatsFloat TowerSpawnTime = new StatsFloat();

	public StatsInt TowersSpawnAutomatically = new StatsInt();

	public StatsInt NumbersOfTowersSpawnAtRunStart = new StatsInt();

	public StatsInt NumberOfProjectilesOfCircle = new StatsInt();

	public StatsInt TowerAoE_NumberOfProjectiles = new StatsInt();

	public StatsFloat TowerGold_GoldMultiplier = new StatsFloat();

	public StatsInt TowerPierce_ChanceToFireBehind = new StatsInt();

	public StatsFloat TowersAttackSpeedMultiplier = new StatsFloat();

	public StatsInt TowerAoE_CanBeFoundInRun = new StatsInt();

	public StatsInt TowerCircle_CanBeFoundInRun = new StatsInt();

	public StatsInt TowerGold_CanBeFoundInRun = new StatsInt();

	public StatsInt TowerPierce_CanBeFoundInRun = new StatsInt();

	public StatsInt TowerQuick_CanBeFoundInRun = new StatsInt();

	public StatsFloat Towers_DamageMultiplier = new StatsFloat();

	public StatsFloat Archer_AttackSpeed = new StatsFloat();

	public StatsFloat Archer_DamageMultiplier = new StatsFloat();

	public StatsInt Archer_Bounce = new StatsInt();

	public StatsInt Archer_Pierce = new StatsInt();

	public StatsInt Archer_Chain = new StatsInt();

	public StatsInt Archer_AdditionalProjectiles = new StatsInt();

	public StatsFloat Archer_ChanceForExplosive = new StatsFloat();

	public StatsFloat Archer_ChanceForPoison = new StatsFloat();

	public StatsFloat Archer_ChanceForIce = new StatsFloat();

	public StatsFloat Well_ChanceForDoubleGold = new StatsFloat();

	public StatsFloat Well_ChanceForDoubleLoot = new StatsFloat();

	public StatsInt Well_BountiesAutoCollect = new StatsInt();

	public StatsInt Well_MonstersSpawnInstantlyIfLessThanX = new StatsInt();

	public StatsFloat Well_ArcherChanceForDoubleArrows = new StatsFloat();

	public StatsFloat Well_ArcherChanceForShootFromBehind = new StatsFloat();

	public StatsInt Well_ItemsCannotBeNormal = new StatsInt();

	public StatsInt Well_ItemsCannotBeRare = new StatsInt();

	public StatsFloat Well_ItemsEffectMultiplier = new StatsFloat();

	public StatsFloat Well_SkillsChanceForEmpoweredEffects = new StatsFloat();

	public StatsInt Well_ShinyCanBeChosenToFarm = new StatsInt();

	public StatsFloat Well_ChosenShinyEffectMultiplier = new StatsFloat();

	public StatsInt Well_UnlockLevelSixForGems = new StatsInt();

	public StatsFloat Well_GemsFirstLevelsEffectMultiplier = new StatsFloat();

	public StatsInt Well_SpawnAllTowersTypesAtRunStart = new StatsInt();

	public StatsFloat SkillsCooldown = new StatsFloat();

	public StatsFloat SkillsChanceForAnotherTrigger = new StatsFloat();

	public StatsInt SkillCurrencyDrop = new StatsInt();

	public StatsFloat SkillsChanceToTriggerOnMonsterDeath = new StatsFloat();

	public StatsFloat Skills_DamageMultiplier = new StatsFloat();

	public StatsFloat CallAllSpecialSkillsEveryXSeconds = new StatsFloat();

	public StatsInt RoA_CallCount = new StatsInt();

	public StatsFloat RoA_DamageIncreaseForEachCallCount = new StatsFloat();

	public StatsFloat RoA_ChanceForTwinFall = new StatsFloat();

	public StatsFloat RoA_ChanceForDragon = new StatsFloat();

	public StatsFloat Dragon_Time = new StatsFloat();

	public StatsFloat Dragon_DamageIncreasedGainedEachSecond = new StatsFloat();

	public StatsFloat Dragon_ChanceToResetAfterFinishing = new StatsFloat();

	public StatsFloat Dragon_RadiusOfEffect = new StatsFloat();

	public StatsInt Knight_CallCount = new StatsInt();

	public StatsFloat Knight_DamageIncreaseForEachCallCount = new StatsFloat();

	public StatsFloat Knight_RadiusOfEffect = new StatsFloat();

	public StatsFloat Knight_ChanceForSpear = new StatsFloat();

	public StatsInt Spear_ChainCount = new StatsInt();

	public StatsFloat Spear_ChanceToForkAfterEachChain = new StatsFloat();

	public StatsFloat Spear_DamageIncreasePerChain = new StatsFloat(new MinMax(0.1, 100.0));

	public StatsFloat Spear_ChanceForTwinFall = new StatsFloat();

	public StatsInt LC_ChainCount = new StatsInt();

	public StatsFloat LC_ChanceToForkAfterEachChain = new StatsFloat();

	public StatsFloat LC_DamageIncreasePerChain = new StatsFloat(new MinMax(0.1, 100.0));

	public StatsFloat LC_ChanceForLightningStrike = new StatsFloat();

	public StatsFloat LightningStrike_RadiusOfEffect = new StatsFloat();

	public StatsFloat LightningStrike_ChanceToDealTripleDamage = new StatsFloat();

	public StatsInt Vampire_CallCount = new StatsInt();

	public StatsFloat Vampire_DamageIncreaseForEachCallCount = new StatsFloat();

	public StatsFloat Vampire_RadiusOfEffect = new StatsFloat();

	public StatsFloat Vampire_ChanceForBloodDrain = new StatsFloat();

	public StatsInt BloodDrain_ChainCount = new StatsInt();

	public StatsFloat BloodDrain_ChanceToForkAfterEachChain = new StatsFloat();

	public StatsFloat BloodDrain_DamageIncreasePerChain = new StatsFloat(new MinMax(0.1, 100.0));

	public StatsFloat BloodDrain_Time = new StatsFloat();

	public StatsFloat BloodDrain_DamageIncreasedGainedEachSecond = new StatsFloat();

	public StatsFloat BloodDrain_ChanceToResetAfterFinishing = new StatsFloat();

	private List<string> DPSStats = new List<string>();

	public event Action OnStatsUpgrade;

	public event Action OnAttackSpeedUpgrade;

	public event Action OnHealthChange;

	public void InitData()
	{
		InitList();
		Init();
		ChangeAStat("Damage", StatsProperties.Flat, DatabaseManager.BaseDamage, IsAdd: true);
		ChangeAStat("MouseRadius", StatsProperties.Flat, DatabaseManager.BaseMouseRadius, IsAdd: true);
		ChangeAStat("MouseAttackSpeed", StatsProperties.Flat, DatabaseManager.BaseMouseAttackSpeed, IsAdd: true);
		ChangeAStat("Timer", StatsProperties.Flat, DatabaseManager.BaseTimer, IsAdd: true);
		ChangeAStat("GoldGained", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("ExpGained", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("NumberOfMonstersSpawned", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("RoA_CallCount", StatsProperties.Flat, 3.0, IsAdd: true);
		ChangeAStat("Dragon_Time", StatsProperties.Flat, 3.0, IsAdd: true);
		ChangeAStat("Dragon_RadiusOfEffect", StatsProperties.Flat, 11.0, IsAdd: true);
		ChangeAStat("Knight_CallCount", StatsProperties.Flat, 2.0, IsAdd: true);
		ChangeAStat("Knight_RadiusOfEffect", StatsProperties.Flat, 11.0, IsAdd: true);
		ChangeAStat("Spear_ChainCount", StatsProperties.Flat, 3.0, IsAdd: true);
		ChangeAStat("LC_ChainCount", StatsProperties.Flat, 3.0, IsAdd: true);
		ChangeAStat("LightningStrike_RadiusOfEffect", StatsProperties.Flat, 10.0, IsAdd: true);
		ChangeAStat("Vampire_CallCount", StatsProperties.Flat, 3.0, IsAdd: true);
		ChangeAStat("Vampire_RadiusOfEffect", StatsProperties.Flat, 11.0, IsAdd: true);
		ChangeAStat("BloodDrain_ChainCount", StatsProperties.Flat, 3.0, IsAdd: true);
		ChangeAStat("BloodDrain_Time", StatsProperties.Flat, 3.0, IsAdd: true);
		ChangeAStat("Skills_DamageMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("NumberOfMonsters", StatsProperties.Flat, DatabaseManager.BaseNumberOfMonsters, IsAdd: true);
		ChangeAStat("SkillsCooldown", StatsProperties.Flat, DatabaseManager.BaseSkillsCooldown, IsAdd: true);
		ChangeAStat("SkillCurrencyDrop", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("BountiesEffect", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("Archer_AttackSpeed", StatsProperties.Flat, DatabaseManager.BaseArcher_AttackSpeed, IsAdd: true);
		ChangeAStat("Archer_DamageMultiplier", StatsProperties.Flat, DatabaseManager.BaseArcher_DamageMultiplier, IsAdd: true);
		ChangeAStat("NumberOfIdleMouseAttacks", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("TriggersOfIdleMouseAttacks", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("MouseAttack_DamageMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("RagedMouseAreaMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("RagedMouseAreaMultiplier", StatsProperties.Additive, 50.0, IsAdd: true);
		ChangeAStat("RagedMouseDamageMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("RagedMouseDamageMultiplier", StatsProperties.Additive, 50.0, IsAdd: true);
		ChangeAStat("MouseProjectile_DamageMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("ChanceToSpawnDebuff", StatsProperties.Flat, DatabaseManager.BaseChanceToSpawnDebuff, IsAdd: true);
		ChangeAStat("DebuffRadius", StatsProperties.Flat, DatabaseManager.BaseDebuffRadius, IsAdd: true);
		ChangeAStat("Debuff_DamageMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("Debuff_DamageMultiplier", StatsProperties.Additive, 50.0, IsAdd: true);
		ChangeAStat("Debuff_GoldMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("Debuff_GoldMultiplier", StatsProperties.Additive, 50.0, IsAdd: true);
		ChangeAStat("Debuff_SkillChance", StatsProperties.Flat, DatabaseManager.BaseDebuff_SkillChance, IsAdd: true);
		ChangeAStat("NumbersOfTowersSpawnAtRunStart", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("NumberOfProjectilesOfCircle", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("TowerAoE_NumberOfProjectiles", StatsProperties.Flat, DatabaseManager.BaseTowerAoE_NumberOfProjectiles, IsAdd: true);
		ChangeAStat("TowerGold_GoldMultiplier", StatsProperties.Flat, DatabaseManager.BaseTowerGold_GoldMultiplier, IsAdd: true);
		ChangeAStat("TowersAttackSpeedMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("Towers_DamageMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("TowerQuick_CanBeFoundInRun", StatsProperties.Flat, 100.0, IsAdd: true);
		ChangeAStat("Well_ItemsEffectMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("Well_ChosenShinyEffectMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
		ChangeAStat("Well_GemsFirstLevelsEffectMultiplier", StatsProperties.Flat, 1.0, IsAdd: true);
	}

	public override void InitList()
	{
	}

	public override void ChangeAStat(string StatFunctionName, StatsProperties SProp, double Value, bool IsAdd)
	{
		if (StatFunctionName.Contains("Well_GemsFirstLevelsEffectMultiplier") && GemsManager.instance != null)
		{
			GemsManager.instance.PurchasedResetEffectOfGems(isAfter: false);
		}
		if (StatFunctionName.Contains("Well_ChosenShinyEffectMultiplier") && ShinyManager.instance != null)
		{
			ShinyManager.instance.PurchasedSelectShinyEffectMultiplier(isAfter: false);
		}
		base.ChangeAStat(StatFunctionName, SProp, Value, IsAdd);
		if (StatFunctionName.Contains("AttackSpeed"))
		{
			this.OnAttackSpeedUpgrade?.Invoke();
		}
		if (StatFunctionName.Contains("Cost") && PlayerManager.instance != null)
		{
			PlayerManager.instance.TriggerEventsFromOutside(Currencies.Gold);
		}
		if (StatFunctionName.Contains("MouseRadius") && MouseAttacker.instance != null)
		{
			MouseAttacker.instance.ChangeMouseSize(0);
		}
		if (StatFunctionName.Contains("Well_GemsFirstLevelsEffectMultiplier") && GemsManager.instance != null)
		{
			GemsManager.instance.PurchasedResetEffectOfGems(isAfter: true);
		}
		if (StatFunctionName.Contains("Well_UnlockLevelSixForGems") && GemsManager.instance != null)
		{
			GemsManager.instance.UnlockOrLockLevelSixForGems();
		}
		if (StatFunctionName.Contains("Well_ShinyCanBeChosenToFarm") && ShinyManager.instance != null)
		{
			ShinyManager.instance.PurchasedSelectShiny();
		}
		if (StatFunctionName.Contains("Well_ChosenShinyEffectMultiplier") && ShinyManager.instance != null)
		{
			ShinyManager.instance.PurchasedSelectShinyEffectMultiplier(isAfter: true);
		}
		if (StatFunctionName.Contains("TowerAoE_CanBeFoundInRun"))
		{
			playerData.instance.isJustUnlockedAoETower = true;
		}
		if (StatFunctionName.Contains("TowerGold_CanBeFoundInRun"))
		{
			playerData.instance.isJustUnlockedGoldTower = true;
		}
		if (StatFunctionName.Contains("TowerPierce_CanBeFoundInRun"))
		{
			playerData.instance.isJustUnlockedPierceTower = true;
		}
		if (StatFunctionName.Contains("TowerCircle_CanBeFoundInRun"))
		{
			playerData.instance.isJustUnlockedCircleTower = true;
		}
		if (StatFunctionName.Contains("ShinyRare_CanBeFoundInRun"))
		{
			playerData.instance.isJustUnlockedRareShiny = true;
		}
		if (StatFunctionName.Contains("ShinyEpic_CanBeFoundInRun"))
		{
			playerData.instance.isJustUnlockedEpicShiny = true;
		}
		if (StatFunctionName.Contains("SkillCurrencyDrop") && SkillCurrencyDrop.Total.RealValue >= 2)
		{
			int playerLevel = playerData.instance.PlayerLevel;
			PlayerManager.instance.ChangeCurrency(Currencies.LevelPoints, playerLevel);
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.LevelPoints] += playerLevel;
			SkillsUIManager.instance.UpdateAllSkillUI();
		}
		if (StatsViewManager.instance != null)
		{
			StatsViewManager.instance.UpdateStatsView();
		}
		this.OnStatsUpgrade?.Invoke();
	}

	public DamageData DamageCalculation(string source, string moreinfo, double multiplier = 1.0)
	{
		double realValue = Damage.Total.RealValue;
		multiplier *= SkillsMultiplier(source, moreinfo);
		if (FunctionsNeeded.IsHappened(ChanceForDoubleDamage.Total.RealValue))
		{
			multiplier *= 2.0;
		}
		if (FunctionsNeeded.IsHappened(ChanceForTripleDamage.Total.RealValue))
		{
			multiplier *= 3.0;
		}
		if (source == "mouse" && moreinfo == "damage2")
		{
			multiplier *= (double)MouseProjectile_DamageMultiplier.Total.RealValue;
		}
		multiplier *= (double)(1f + DamageMultiplier_PerTotalShinyFound.Total.RealValue * (float)playerData.instance.TotalShinyFound / 100f);
		multiplier *= (double)(1f + DamageMultiplier_PerStatsInAllItemsEquipped.Total.RealValue * (float)playerData.instance.TotalStatsInItemsEquipped / 100f);
		multiplier *= (double)(1f + DamageMultiplier_PerBountyFound.Total.RealValue * (float)playerData.instance.TotalBountiesFound_CurrentRun / 100f);
		multiplier *= (double)(1f + DamageMultiplier_PerLevelOfGems.Total.RealValue * (float)playerData.instance.TotalGemsLeveledUp / 100f);
		multiplier *= (double)(1f + DamageMultiplier_PerSecondInTimer.Total.RealValue * Timer.Total.RealValue / 100f);
		multiplier *= (double)(1f + DamageMultiplier_PerAreaMarkApplied.Total.RealValue * (float)playerData.instance.TotalAreaMarksApplied_CurrentRun / 100f);
		multiplier *= (double)(1f + DamageMultiplier_PerMonster.Total.RealValue * (float)NumberOfMonsters.Total.RealValue / 100f);
		return new DamageData(Math.Ceiling(realValue * multiplier), IsCrit: false, source);
	}

	private double SkillsMultiplier(string source, string moreinfo)
	{
		double num = 1.0;
		if (DatabaseManager.ProjectileDict.ContainsKey(source))
		{
			ProjectileInfo projectileInfo = DatabaseManager.ProjectileDict[source];
			if (projectileInfo.Parameters.ContainsKey(moreinfo))
			{
				num = projectileInfo.Parameters[moreinfo];
			}
		}
		switch (source)
		{
		case "Archer1_Projectile_RoA":
		case "Knight1_Projectile_AreaSlash":
		case "Vampire1_Projectile_VampireExp":
			num *= (double)(1f + (float)GetCallCount(source) * GetDamageIncreaseForEachCallCount(source) / 100f);
			break;
		case "Lightning1_Projectile_LChain":
		case "Knight2_Projectile_Spear":
		case "Vampire2_Projectile_BloodSuck":
			num *= (double)(1f + (float)GetChainCount(source) * GetDamageIncreasePerChain(source) / 100f);
			break;
		}
		switch (source)
		{
		case "Archer1_Projectile_RoA":
		case "Knight1_Projectile_AreaSlash":
		case "Vampire1_Projectile_VampireExp":
		case "Lightning1_Projectile_LChain":
		case "Knight2_Projectile_Spear":
		case "Vampire2_Projectile_BloodSuck":
		case "Lightning2_Projectile_LStrike":
		case "Archer2_Projectile_Dragon":
			num *= (double)Skills_DamageMultiplier.Total.RealValue;
			break;
		}
		return num;
	}

	public int GetCallCount(string skillName)
	{
		switch (skillName)
		{
		case "Archer1_Projectile_RoA":
			return RoA_CallCount.Total.RealValue;
		case "Knight1_Projectile_AreaSlash":
			return Knight_CallCount.Total.RealValue;
		case "Vampire1_Projectile_VampireExp":
			return Vampire_CallCount.Total.RealValue;
		default:
			UnityEngine.Debug.LogError("No Call Count for " + skillName);
			return 1;
		}
	}

	public float GetDamageIncreaseForEachCallCount(string skillName)
	{
		switch (skillName)
		{
		case "Archer1_Projectile_RoA":
			return RoA_DamageIncreaseForEachCallCount.Total.RealValue;
		case "Knight1_Projectile_AreaSlash":
			return Knight_DamageIncreaseForEachCallCount.Total.RealValue;
		case "Vampire1_Projectile_VampireExp":
			return Vampire_DamageIncreaseForEachCallCount.Total.RealValue;
		default:
			UnityEngine.Debug.LogError("No Damage Increase For Each Call Count for " + skillName);
			return 0f;
		}
	}

	public float ChanceForTwinFall(string skillName)
	{
		if (skillName == "Archer1_Projectile_RoA")
		{
			return RoA_ChanceForTwinFall.Total.RealValue;
		}
		if (skillName == "Knight2_Projectile_Spear")
		{
			return Spear_ChanceForTwinFall.Total.RealValue;
		}
		UnityEngine.Debug.LogError("No Chance For Twin Fall for " + skillName);
		return 0f;
	}

	public int GetChainCount(string skillName)
	{
		switch (skillName)
		{
		case "Lightning1_Projectile_LChain":
			return LC_ChainCount.Total.RealValue;
		case "Knight2_Projectile_Spear":
			return Spear_ChainCount.Total.RealValue;
		case "Vampire2_Projectile_BloodSuck":
			return BloodDrain_ChainCount.Total.RealValue;
		default:
			UnityEngine.Debug.LogError("No Chain Count for " + skillName);
			return 3;
		}
	}

	public float GetChanceToForkAfterEachChain(string skillName)
	{
		switch (skillName)
		{
		case "Lightning1_Projectile_LChain":
			return LC_ChanceToForkAfterEachChain.Total.RealValue;
		case "Knight2_Projectile_Spear":
			return Spear_ChanceToForkAfterEachChain.Total.RealValue;
		case "Vampire2_Projectile_BloodSuck":
			return BloodDrain_ChanceToForkAfterEachChain.Total.RealValue;
		default:
			UnityEngine.Debug.LogError("No Chance To Fork After Each Chain for " + skillName);
			return 0f;
		}
	}

	public float GetDamageIncreasePerChain(string skillName)
	{
		switch (skillName)
		{
		case "Lightning1_Projectile_LChain":
			return LC_DamageIncreasePerChain.Total.RealValue;
		case "Knight2_Projectile_Spear":
			return Spear_DamageIncreasePerChain.Total.RealValue;
		case "Vampire2_Projectile_BloodSuck":
			return BloodDrain_DamageIncreasePerChain.Total.RealValue;
		default:
			UnityEngine.Debug.LogError("No Damage Increase Per Chain for " + skillName);
			return 0f;
		}
	}

	public float GetRadiusOfEffectArea(string skillName)
	{
		switch (skillName)
		{
		case "Knight1_Projectile_AreaSlash":
			return Knight_RadiusOfEffect.Total.RealValue;
		case "Vampire1_Projectile_VampireExp":
			return Vampire_RadiusOfEffect.Total.RealValue;
		case "Lightning2_Projectile_LStrike":
			return LightningStrike_RadiusOfEffect.Total.RealValue;
		case "Archer2_Projectile_Dragon":
			return Dragon_RadiusOfEffect.Total.RealValue;
		default:
			UnityEngine.Debug.LogError("No Radius Of Effect Area for " + skillName);
			return 5f;
		}
	}

	public float GetTime(string skillName)
	{
		if (skillName == "Archer2_Projectile_Dragon")
		{
			return Dragon_Time.Total.RealValue;
		}
		if (skillName == "Vampire2_Projectile_BloodSuck")
		{
			return BloodDrain_Time.Total.RealValue;
		}
		UnityEngine.Debug.LogError("No Time for " + skillName);
		return 3f;
	}

	public float GetDamageIncreasedGainedEachSecond(string skillName)
	{
		if (skillName == "Archer2_Projectile_Dragon")
		{
			return Dragon_DamageIncreasedGainedEachSecond.Total.RealValue;
		}
		if (skillName == "Vampire2_Projectile_BloodSuck")
		{
			return BloodDrain_DamageIncreasedGainedEachSecond.Total.RealValue;
		}
		UnityEngine.Debug.LogError("No Damage Increased Gained Each Second for " + skillName);
		return 0f;
	}

	public float GetChanceToResetAfterFinishing(string skillName)
	{
		if (skillName == "Archer2_Projectile_Dragon")
		{
			return Dragon_ChanceToResetAfterFinishing.Total.RealValue;
		}
		if (skillName == "Vampire2_Projectile_BloodSuck")
		{
			return BloodDrain_ChanceToResetAfterFinishing.Total.RealValue;
		}
		UnityEngine.Debug.LogError("No Chance To Reset After Finishing for " + skillName);
		return 0f;
	}
}
[CreateAssetMenu]
public class StatInfo : ScriptableObject
{
	public string VariableName;

	public StatsProperties StatsProp;

	public ValueNumberTypes ValueNumberType;

	public int RoundToNearest;

	public bool isNormalyStartsWithSign;

	public bool isList;

	public bool isHasTotalVersion;

	[HideInInspector]
	public string functionName => VariableName + StatsProp;

	public string GetValueDescText(bool IsStartsWithSign, double TheValue, string statVersion = "")
	{
		string text = "";
		if (statVersion == "")
		{
			statVersion = StatsProp.ToString();
		}
		string text2 = "";
		if (IsStartsWithSign)
		{
			text2 = ((!(TheValue < 0.0)) ? "+" : "-");
		}
		if (ValueNumberType == ValueNumberTypes.Int || ValueNumberType == ValueNumberTypes.Float || (ValueNumberType == ValueNumberTypes.Double && statVersion != "Flat"))
		{
			DoubleAndRoundToNearest value = new DoubleAndRoundToNearest(Math.Abs(TheValue), RoundToNearest, text2);
			return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + statVersion, value) + text;
		}
		if (ValueNumberType == ValueNumberTypes.Double)
		{
			return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + statVersion, text2 + TheValue.ToReadable()) + text;
		}
		return "";
	}

	public string GetValueDescText_SingleOrMultipleValues(List<double> values, bool isColoredTag)
	{
		List<double> list = new List<double>(values);
		if (VariableName.Contains("Well_SkillsChanceForEmpoweredEffects"))
		{
			list.Add(DatabaseManager.NumberOfEffectsForEmpoweredSkills);
		}
		if (VariableName.Contains("ChanceToManyXGoldGained"))
		{
			list.Add(DatabaseManager.XAmountToManyXGoldGained);
		}
		if (VariableName.Contains("TimerOnMonsterDeath"))
		{
			list.Add(DatabaseManager.MaxTimerOnMonsterDeath);
		}
		string coloredTag = "";
		if (list.Count == 1)
		{
			if (isColoredTag)
			{
				coloredTag = "#VALUE#";
			}
			return GetValueDescText_MultipleValues("#VALUE#", new Dictionary<string, double> { 
			{
				"#VALUE#",
				list[0]
			} }, new Dictionary<string, string>(), coloredTag);
		}
		Dictionary<string, double> dictionary = new Dictionary<string, double>();
		if (isColoredTag)
		{
			coloredTag = "#VALUE1#";
		}
		for (int i = 0; i < list.Count; i++)
		{
			dictionary.Add("#VALUE" + (i + 1) + "#", list[i]);
		}
		return GetValueDescText_MultipleValues("#VALUE1#", dictionary, new Dictionary<string, string>(), coloredTag);
	}

	public string GetValueDescText_MultipleValues(string mainNumberTag, Dictionary<string, double> NumberValues, Dictionary<string, string> StringValues, string ColoredTag)
	{
		string text = "";
		string text2 = StatsProp.ToString();
		string sign = "";
		Dictionary<string, DoubleAndRoundToNearest> dictionary = new Dictionary<string, DoubleAndRoundToNearest>();
		foreach (KeyValuePair<string, double> NumberValue in NumberValues)
		{
			if (isNormalyStartsWithSign)
			{
				sign = ((!(NumberValue.Value < 0.0)) ? "+" : "-");
			}
			DoubleAndRoundToNearest value = new DoubleAndRoundToNearest(Math.Abs(NumberValue.Value), RoundToNearest, sign);
			dictionary.Add(NumberValue.Key, value);
		}
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> StringValue in StringValues)
		{
			dictionary2.Add(StringValue.Key, StringValue.Value);
		}
		foreach (string key in dictionary.Keys)
		{
			if (ValueNumberType == ValueNumberTypes.Int || ValueNumberType == ValueNumberTypes.Float)
			{
				double number = Math.Round(dictionary[key].DoubleValue, dictionary[key].RoundToNearest);
				string text3 = dictionary[key].StartingSign + number.ToReadable(isForceReadable: false, dictionary[key].RoundToNearest);
				if (ColoredTag == key)
				{
					text3 = "<b><color=#" + FunctionsNeeded.ColorToHex(ManagerOfTheGame.instance.TreeMainStatColor) + ">" + text3 + "</color></b>";
				}
				dictionary2.Add(key, text3);
			}
			else if (ValueNumberType == ValueNumberTypes.Double)
			{
				if (ColoredTag == key)
				{
					string value2 = "<b><color=#" + FunctionsNeeded.ColorToHex(ManagerOfTheGame.instance.TreeMainStatColor) + ">" + dictionary[key].DoubleValue.ToReadable() + "</color></b>";
					dictionary2.Add(key, value2);
				}
				else if (key == mainNumberTag)
				{
					string value3 = dictionary[key].StartingSign + dictionary[key].DoubleValue.ToReadable();
					dictionary2.Add(key, value3);
				}
				else
				{
					double number2 = Math.Round(dictionary[key].DoubleValue, dictionary[key].RoundToNearest);
					string value4 = dictionary[key].StartingSign + number2.ToReadable(isForceReadable: false, dictionary[key].RoundToNearest);
					dictionary2.Add(key, value4);
				}
			}
		}
		if (NumberValues.Count > 0)
		{
			_ = StringValues.Count;
			_ = 0;
			return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + text2, dictionary2) + text;
		}
		return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + text2, dictionary2) + text;
	}

	public string GetValueDescText_ForTextsValues(string TheValue, string statVersion = "")
	{
		string text = "";
		if (statVersion == "")
		{
			statVersion = StatsProp.ToString();
		}
		return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + statVersion, TheValue) + text;
	}

	public string GetValueDescText(double TheValue, string statVersion = "")
	{
		return GetValueDescText(isNormalyStartsWithSign, TheValue, statVersion);
	}

	public string GetValueDescText(DoubleAndRoundToNearest ValueAndRounder)
	{
		if (ValueNumberType == ValueNumberTypes.Int)
		{
			return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + StatsProp, ValueAndRounder);
		}
		if (ValueNumberType == ValueNumberTypes.Float)
		{
			return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + StatsProp, ValueAndRounder);
		}
		if (ValueNumberType == ValueNumberTypes.Double)
		{
			return LocalizerManager.GetTranslatedThenReplaceValues(VariableName + StatsProp, ValueAndRounder.StartingSign + ValueAndRounder.DoubleValue.ToReadable());
		}
		return "";
	}
}
public enum ValueNumberTypes
{
	Int,
	Float,
	Double,
	Bool
}
[Serializable]
public class StatsData
{
	[JsonIgnore]
	public Dictionary<string, object> StatsDict = new Dictionary<string, object>();

	[JsonIgnore]
	public Dictionary<string, int> ListsNamesAndCount = new Dictionary<string, int>();

	public virtual void InitList()
	{
	}

	public void Init()
	{
		StatsDict.Clear();
		ListsNamesAndCount.Clear();
		CreateTheStatsDict();
	}

	public void CreateTheStatsDict()
	{
		FieldInfo[] fields = GetType().GetFields();
		for (int i = 0; i < fields.Length; i++)
		{
			if (fields[i].FieldType == typeof(StatsInt) || fields[i].FieldType == typeof(StatsFloat) || fields[i].FieldType == typeof(StatsDouble) || fields[i].FieldType == typeof(StatsBool))
			{
				StatsDict.Add(fields[i].Name, fields[i].GetValue(this));
			}
			else if (fields[i].FieldType == typeof(List<StatsInt>))
			{
				List<StatsInt> list = (List<StatsInt>)fields[i].GetValue(this);
				if (list != null)
				{
					ListsNamesAndCount.Add(fields[i].Name, list.Count);
					for (int j = 0; j < list.Count; j++)
					{
						StatsDict.Add(fields[i].Name + j, list[j]);
					}
				}
			}
			else if (fields[i].FieldType == typeof(List<StatsFloat>))
			{
				List<StatsFloat> list2 = (List<StatsFloat>)fields[i].GetValue(this);
				if (list2 != null)
				{
					ListsNamesAndCount.Add(fields[i].Name, list2.Count);
					for (int k = 0; k < list2.Count; k++)
					{
						StatsDict.Add(fields[i].Name + k, list2[k]);
					}
				}
			}
			else if (fields[i].FieldType == typeof(List<StatsDouble>))
			{
				List<StatsDouble> list3 = (List<StatsDouble>)fields[i].GetValue(this);
				if (list3 != null)
				{
					ListsNamesAndCount.Add(fields[i].Name, list3.Count);
					for (int l = 0; l < list3.Count; l++)
					{
						StatsDict.Add(fields[i].Name + l, list3[l]);
					}
				}
			}
			else if (fields[i].FieldType == typeof(List<StatsBool>))
			{
				List<StatsBool> list4 = (List<StatsBool>)fields[i].GetValue(this);
				if (list4 != null)
				{
					ListsNamesAndCount.Add(fields[i].Name, list4.Count);
					for (int m = 0; m < list4.Count; m++)
					{
						StatsDict.Add(fields[i].Name + m, list4[m]);
					}
				}
			}
			else
			{
				if (!(fields[i].FieldType == typeof(StringStatsFloat)))
				{
					continue;
				}
				StringStatsFloat stringStatsFloat = (StringStatsFloat)fields[i].GetValue(this);
				if (stringStatsFloat == null)
				{
					continue;
				}
				ListsNamesAndCount.Add(fields[i].Name, stringStatsFloat.Count);
				foreach (KeyValuePair<string, StatsFloat> item in stringStatsFloat)
				{
					StatsDict.Add(fields[i].Name + item.Key, item.Value);
				}
			}
		}
	}

	public void CreateTheStatsDict_ForListsThatAddedMoreElementsInNewPatches()
	{
		FieldInfo[] fields = GetType().GetFields();
		for (int i = 0; i < fields.Length; i++)
		{
			if (fields[i].FieldType == typeof(List<StatsInt>))
			{
				List<StatsInt> list = (List<StatsInt>)fields[i].GetValue(this);
				if (list == null)
				{
					continue;
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (!StatsDict.ContainsKey(fields[i].Name + j))
					{
						StatsDict.Add(fields[i].Name + j, list[j]);
						ListsNamesAndCount[fields[i].Name]++;
					}
				}
			}
			else if (fields[i].FieldType == typeof(List<StatsFloat>))
			{
				List<StatsFloat> list2 = (List<StatsFloat>)fields[i].GetValue(this);
				if (list2 == null)
				{
					continue;
				}
				for (int k = 0; k < list2.Count; k++)
				{
					if (!StatsDict.ContainsKey(fields[i].Name + k))
					{
						StatsDict.Add(fields[i].Name + k, list2[k]);
						ListsNamesAndCount[fields[i].Name]++;
					}
				}
			}
			else if (fields[i].FieldType == typeof(List<StatsDouble>))
			{
				List<StatsDouble> list3 = (List<StatsDouble>)fields[i].GetValue(this);
				if (list3 == null)
				{
					continue;
				}
				for (int l = 0; l < list3.Count; l++)
				{
					if (!StatsDict.ContainsKey(fields[i].Name + l))
					{
						StatsDict.Add(fields[i].Name + l, list3[l]);
						ListsNamesAndCount[fields[i].Name]++;
					}
				}
			}
			else if (fields[i].FieldType == typeof(List<StatsBool>))
			{
				List<StatsBool> list4 = (List<StatsBool>)fields[i].GetValue(this);
				if (list4 == null)
				{
					continue;
				}
				for (int m = 0; m < list4.Count; m++)
				{
					if (!StatsDict.ContainsKey(fields[i].Name + m))
					{
						StatsDict.Add(fields[i].Name + m, list4[m]);
						ListsNamesAndCount[fields[i].Name]++;
					}
				}
			}
			else
			{
				if (!(fields[i].FieldType == typeof(StringStatsFloat)))
				{
					continue;
				}
				StringStatsFloat stringStatsFloat = (StringStatsFloat)fields[i].GetValue(this);
				if (stringStatsFloat == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, StatsFloat> item in stringStatsFloat)
				{
					if (!StatsDict.ContainsKey(fields[i].Name + item.Key))
					{
						StatsDict.Add(fields[i].Name + item.Key, item.Value);
						ListsNamesAndCount[fields[i].Name]++;
					}
				}
			}
		}
	}

	public virtual void ChangeAStat(string StatFunctionName, StatsProperties SProp, double Value, bool IsAdd)
	{
		if (ListsNamesAndCount.ContainsKey(StatFunctionName))
		{
			for (int i = 0; i < ListsNamesAndCount[StatFunctionName]; i++)
			{
				ChangeAStat(StatFunctionName + i, SProp, Value, IsAdd);
			}
		}
		else if (StatsDict[StatFunctionName].GetType() == typeof(StatsInt))
		{
			ChangeStatsInt((StatsInt)StatsDict[StatFunctionName], SProp, (float)Value, IsAdd);
		}
		else if (StatsDict[StatFunctionName].GetType() == typeof(StatsFloat))
		{
			ChangeStatsFloat((StatsFloat)StatsDict[StatFunctionName], SProp, (float)Value, IsAdd);
		}
		else if (StatsDict[StatFunctionName].GetType() == typeof(StatsDouble))
		{
			ChangeStatsDouble((StatsDouble)StatsDict[StatFunctionName], SProp, Value, IsAdd);
		}
		else if (StatsDict[StatFunctionName].GetType() == typeof(StatsBool))
		{
			ChangeStatsBool((StatsBool)StatsDict[StatFunctionName], SProp, Value, IsAdd);
		}
	}

	protected void ChangeStatsInt(StatsInt TheObj, StatsProperties SProp, float Value, bool IsAdd)
	{
		switch (SProp)
		{
		case StatsProperties.Flat:
			TheObj.Flat.SetOnlyValue += (int)Value * (IsAdd ? 1 : (-1));
			break;
		case StatsProperties.Additive:
			TheObj.AdditiveIncreases.SetOnlyValue += Value * (float)(IsAdd ? 1 : (-1));
			break;
		case StatsProperties.Multiplicative:
			TheObj.MultiplicativeIncreases.SetOnlyValue *= (float)Math.Pow(1f + Value / 100f, IsAdd ? 1 : (-1));
			break;
		case StatsProperties.SetFlat_And_ResetAddMulti:
			TheObj.Flat.SetOnlyValue = (int)Value;
			TheObj.AdditiveIncreases.SetOnlyValue = 0f;
			TheObj.MultiplicativeIncreases.SetOnlyValue = 1f;
			break;
		case StatsProperties.SetFlatOnly:
			TheObj.Flat.SetOnlyValue = (int)Value;
			break;
		case StatsProperties.SetAdditiveOnly:
			TheObj.AdditiveIncreases.SetOnlyValue = Value;
			break;
		case StatsProperties.SetMultiplicativeOnly:
			TheObj.MultiplicativeIncreases.SetOnlyValue = Value;
			break;
		}
		TheObj.CalculateTotal();
	}

	protected void ChangeStatsFloat(StatsFloat TheObj, StatsProperties SProp, float Value, bool IsAdd)
	{
		switch (SProp)
		{
		case StatsProperties.Flat:
			TheObj.Flat.SetOnlyValue += Value * (float)(IsAdd ? 1 : (-1));
			break;
		case StatsProperties.Additive:
			TheObj.AdditiveIncreases.SetOnlyValue += Value * (float)(IsAdd ? 1 : (-1));
			break;
		case StatsProperties.Multiplicative:
			TheObj.MultiplicativeIncreases.SetOnlyValue *= (float)Math.Pow(1f + Value / 100f, IsAdd ? 1 : (-1));
			break;
		case StatsProperties.SetFlat_And_ResetAddMulti:
			TheObj.Flat.SetOnlyValue = Value;
			TheObj.AdditiveIncreases.SetOnlyValue = 0f;
			TheObj.MultiplicativeIncreases.SetOnlyValue = 1f;
			break;
		case StatsProperties.SetFlatOnly:
			TheObj.Flat.SetOnlyValue = Value;
			break;
		case StatsProperties.SetAdditiveOnly:
			TheObj.AdditiveIncreases.SetOnlyValue = Value;
			break;
		case StatsProperties.SetMultiplicativeOnly:
			TheObj.MultiplicativeIncreases.SetOnlyValue = Value;
			break;
		}
		TheObj.CalculateTotal();
	}

	protected void ChangeStatsDouble(StatsDouble TheObj, StatsProperties SProp, double Value, bool IsAdd)
	{
		switch (SProp)
		{
		case StatsProperties.Flat:
			TheObj.Flat.SetOnlyValue += Value * (double)(IsAdd ? 1 : (-1));
			break;
		case StatsProperties.Additive:
			TheObj.AdditiveIncreases.SetOnlyValue += Value * (double)(IsAdd ? 1 : (-1));
			break;
		case StatsProperties.Multiplicative:
			TheObj.MultiplicativeIncreases.SetOnlyValue *= Math.Pow(1.0 + Value / 100.0, IsAdd ? 1 : (-1));
			break;
		case StatsProperties.SetFlat_And_ResetAddMulti:
			TheObj.Flat.SetOnlyValue = Value;
			TheObj.AdditiveIncreases.SetOnlyValue = 0.0;
			TheObj.MultiplicativeIncreases.SetOnlyValue = 1.0;
			break;
		case StatsProperties.SetFlatOnly:
			TheObj.Flat.SetOnlyValue = Value;
			break;
		case StatsProperties.SetAdditiveOnly:
			TheObj.AdditiveIncreases.SetOnlyValue = Value;
			break;
		case StatsProperties.SetMultiplicativeOnly:
			TheObj.MultiplicativeIncreases.SetOnlyValue = Value;
			break;
		}
		TheObj.CalculateTotal();
	}

	protected void ChangeStatsBool(StatsBool TheObj, StatsProperties SProp, double Value, bool IsAdd)
	{
		TheObj.Value = IsAdd;
	}
}
[Serializable]
public class StatsValueInt
{
	[SerializeField]
	private int _Value;

	public int SetOnlyValue
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
			RealValue = FunctionsNeeded.Clamp(_Value, Min, Max);
		}
	}

	public int RealValue { get; set; }

	public int Max { get; set; }

	public int Min { get; set; }

	public StatsValueInt(MinMax MinAndMax)
	{
		Min = (int)MinAndMax.MinimumValue;
		Max = (int)MinAndMax.MaximumValue;
	}

	public StatsValueInt CopyMe()
	{
		return new StatsValueInt(new MinMax(0.0, 1.0))
		{
			Max = Max,
			Min = Min,
			SetOnlyValue = SetOnlyValue
		};
	}
}
[Serializable]
public class StatsValueFloat
{
	[JsonProperty]
	public float _Value;

	[JsonProperty]
	public float RealValue;

	[JsonProperty]
	public float Max;

	[JsonProperty]
	public float Min;

	[JsonProperty]
	public float SetOnlyValue
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
			RealValue = FunctionsNeeded.Clamp(_Value, Min, Max);
		}
	}

	public StatsValueFloat(MinMax MinAndMax)
	{
		Min = (float)MinAndMax.MinimumValue;
		Max = (float)MinAndMax.MaximumValue;
	}

	public StatsValueFloat()
	{
	}

	public StatsValueFloat CopyMe()
	{
		return new StatsValueFloat(new MinMax(0.0, 1.0))
		{
			Max = Max,
			Min = Min,
			SetOnlyValue = SetOnlyValue
		};
	}
}
[Serializable]
public class StatsValueDouble
{
	[SerializeField]
	private double _Value;

	public double SetOnlyValue
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
			if ((_Value > Max && Max < 1E+100) || _Value <= Min)
			{
				RealValue = FunctionsNeeded.Clamp(_Value, Min, Max);
			}
			else
			{
				RealValue = _Value;
			}
		}
	}

	public double RealValue { get; set; }

	public double Max { get; set; }

	public double Min { get; set; }

	public StatsValueDouble()
	{
	}

	public StatsValueDouble(MinMax MinAndMax)
	{
		Min = MinAndMax.MinimumValue;
		Max = MinAndMax.MaximumValue;
	}

	public StatsValueDouble CopyMe()
	{
		return new StatsValueDouble(new MinMax(0.0, 1.0))
		{
			Max = Max,
			Min = Min,
			SetOnlyValue = SetOnlyValue
		};
	}
}
[Serializable]
public class StatsInt
{
	public int MinTotal { get; set; }

	public int MaxTotal { get; set; }

	public int MinFlat { get; set; }

	public int MaxFlat { get; set; }

	public float MinAddi { get; set; }

	public float MaxAddi { get; set; }

	public StatsValueInt Total { get; set; }

	public StatsValueInt Flat { get; set; }

	public StatsValueFloat AdditiveIncreases { get; set; }

	public StatsValueFloat MultiplicativeIncreases { get; set; }

	private void InitStatsValue()
	{
		Flat = new StatsValueInt(new MinMax(MinFlat, MaxFlat));
		Total = new StatsValueInt(new MinMax(MinTotal, MaxTotal));
		AdditiveIncreases = new StatsValueFloat(new MinMax(MinAddi, MaxAddi));
		MultiplicativeIncreases = new StatsValueFloat(new MinMax(MinAddi, MaxAddi));
		MultiplicativeIncreases.SetOnlyValue = 1f;
	}

	public StatsInt()
	{
		MaxTotal = 2147482647;
		MinTotal = 0;
		MaxFlat = 2147482647;
		MinFlat = 0;
		MaxAddi = 2.1474826E+09f;
		MinAddi = -90f;
		InitStatsValue();
	}

	public StatsInt(MinMax MinMaxTotal, MinMax MinMaxFlat, MinMax MinMaxAddi)
	{
		MaxTotal = (int)MinMaxTotal.MaximumValue;
		MinTotal = (int)MinMaxTotal.MinimumValue;
		MaxFlat = (int)MinMaxFlat.MaximumValue;
		MinFlat = (int)MinMaxFlat.MinimumValue;
		MaxAddi = (float)MinMaxAddi.MaximumValue;
		MinAddi = (float)MinMaxAddi.MinimumValue;
		InitStatsValue();
	}

	public StatsInt(MinMax MinMaxTotalAndFlat)
	{
		MaxTotal = (int)MinMaxTotalAndFlat.MaximumValue;
		MinTotal = (int)MinMaxTotalAndFlat.MinimumValue;
		MaxFlat = (int)MinMaxTotalAndFlat.MaximumValue;
		MinFlat = (int)MinMaxTotalAndFlat.MinimumValue;
		MaxAddi = float.MaxValue;
		MinAddi = -90f;
		InitStatsValue();
	}

	public StatsInt CopyMe()
	{
		StatsInt statsInt = new StatsInt();
		statsInt.MinTotal = MinTotal;
		statsInt.MaxTotal = MaxTotal;
		statsInt.MinFlat = MinFlat;
		statsInt.MaxFlat = MaxFlat;
		statsInt.MinAddi = MinAddi;
		statsInt.MaxAddi = MaxAddi;
		statsInt.Total = Total.CopyMe();
		statsInt.Flat = Flat.CopyMe();
		statsInt.AdditiveIncreases = AdditiveIncreases.CopyMe();
		statsInt.MultiplicativeIncreases = MultiplicativeIncreases.CopyMe();
		statsInt.CalculateTotal();
		return statsInt;
	}

	public void CalculateTotal()
	{
		Total.SetOnlyValue = (int)Math.Round((float)Flat.RealValue * (1f + AdditiveIncreases.RealValue / 100f) * MultiplicativeIncreases.RealValue, 0);
	}

	public void ResetAllDataToZero()
	{
		Flat.SetOnlyValue = 0;
		AdditiveIncreases.SetOnlyValue = 0f;
		MultiplicativeIncreases.SetOnlyValue = 1f;
		CalculateTotal();
	}

	public override string ToString()
	{
		return Total.RealValue.ToString();
	}
}
[Serializable]
public class StatsFloat
{
	[JsonProperty]
	public float MinTotal { get; set; }

	[JsonProperty]
	public float MaxTotal { get; set; }

	[JsonProperty]
	public float MinFlat { get; set; }

	[JsonProperty]
	public float MaxFlat { get; set; }

	[JsonProperty]
	public float MinAddi { get; set; }

	[JsonProperty]
	public float MaxAddi { get; set; }

	[JsonProperty]
	public StatsValueFloat Flat { get; set; }

	[JsonProperty]
	public StatsValueFloat Total { get; set; }

	[JsonProperty]
	public StatsValueFloat AdditiveIncreases { get; set; }

	[JsonProperty]
	public StatsValueFloat MultiplicativeIncreases { get; set; }

	private void InitStatsValue()
	{
		Flat = new StatsValueFloat(new MinMax(MinFlat, MaxFlat));
		Total = new StatsValueFloat(new MinMax(MinTotal, MaxTotal));
		AdditiveIncreases = new StatsValueFloat(new MinMax(MinAddi, MaxAddi));
		MultiplicativeIncreases = new StatsValueFloat(new MinMax(MinAddi, MaxAddi));
		MultiplicativeIncreases.SetOnlyValue = 1f;
	}

	public StatsFloat()
	{
		MaxTotal = float.MaxValue;
		MinTotal = 0f;
		MaxFlat = float.MaxValue;
		MinFlat = 0f;
		MaxAddi = float.MaxValue;
		MinAddi = -90f;
		InitStatsValue();
	}

	public StatsFloat(MinMax MinMaxTotal, MinMax MinMaxFlat, MinMax MinMaxAddi)
	{
		MaxTotal = (float)MinMaxTotal.MaximumValue;
		MinTotal = (float)MinMaxTotal.MinimumValue;
		MaxFlat = (float)MinMaxFlat.MaximumValue;
		MinFlat = (float)MinMaxFlat.MinimumValue;
		MaxAddi = (float)MinMaxAddi.MaximumValue;
		MinAddi = (float)MinMaxAddi.MinimumValue;
		InitStatsValue();
	}

	public StatsFloat(MinMax MinMaxTotalAndFlat)
	{
		MaxTotal = (float)MinMaxTotalAndFlat.MaximumValue;
		MinTotal = (float)MinMaxTotalAndFlat.MinimumValue;
		MaxFlat = (float)MinMaxTotalAndFlat.MaximumValue;
		MinFlat = (float)MinMaxTotalAndFlat.MinimumValue;
		MaxAddi = float.MaxValue;
		MinAddi = -90f;
		InitStatsValue();
	}

	public StatsFloat CopyMe()
	{
		StatsFloat statsFloat = new StatsFloat();
		statsFloat.MinTotal = MinTotal;
		statsFloat.MaxTotal = MaxTotal;
		statsFloat.MinFlat = MinFlat;
		statsFloat.MaxFlat = MaxFlat;
		statsFloat.MinAddi = MinAddi;
		statsFloat.MaxAddi = MaxAddi;
		statsFloat.Total = Total.CopyMe();
		statsFloat.Flat = Flat.CopyMe();
		statsFloat.AdditiveIncreases = AdditiveIncreases.CopyMe();
		statsFloat.MultiplicativeIncreases = MultiplicativeIncreases.CopyMe();
		statsFloat.CalculateTotal();
		return statsFloat;
	}

	public void CalculateTotal()
	{
		Total.SetOnlyValue = (float)Math.Round(Flat.RealValue * (1f + AdditiveIncreases.RealValue / 100f) * MultiplicativeIncreases.RealValue, 2);
	}

	public void ResetAllDataToZero()
	{
		Flat.SetOnlyValue = 0f;
		AdditiveIncreases.SetOnlyValue = 0f;
		MultiplicativeIncreases.SetOnlyValue = 1f;
		CalculateTotal();
	}

	public override string ToString()
	{
		return Total.RealValue.ToString("0.00");
	}
}
[Serializable]
public class StatsDouble
{
	public double MinTotal { get; set; }

	public double MaxTotal { get; set; }

	public double MinFlat { get; set; }

	public double MaxFlat { get; set; }

	public double MinAddi { get; set; }

	public double MaxAddi { get; set; }

	public StatsValueDouble Flat { get; set; }

	public StatsValueDouble Total { get; set; }

	public StatsValueDouble AdditiveIncreases { get; set; }

	public StatsValueDouble MultiplicativeIncreases { get; set; }

	private void InitStatsValue()
	{
		Flat = new StatsValueDouble(new MinMax(MinFlat, MaxFlat));
		Total = new StatsValueDouble(new MinMax(MinTotal, MaxTotal));
		AdditiveIncreases = new StatsValueDouble(new MinMax(MinAddi, MaxAddi));
		MultiplicativeIncreases = new StatsValueDouble(new MinMax(MinAddi, MaxAddi));
		MultiplicativeIncreases.SetOnlyValue = 1.0;
	}

	[JsonConstructor]
	public StatsDouble(float MaxFlat)
	{
	}

	public StatsDouble()
	{
		MaxTotal = double.MaxValue;
		MinTotal = 0.0;
		MaxFlat = double.MaxValue;
		MinFlat = 0.0;
		MaxAddi = double.MaxValue;
		MinAddi = -90.0;
		InitStatsValue();
	}

	public StatsDouble(MinMax MinMaxTotal, MinMax MinMaxFlat, MinMax MinMaxAddi)
	{
		MaxTotal = MinMaxTotal.MaximumValue;
		MinTotal = MinMaxTotal.MinimumValue;
		MaxFlat = MinMaxFlat.MaximumValue;
		MinFlat = MinMaxFlat.MinimumValue;
		MaxAddi = MinMaxAddi.MaximumValue;
		MinAddi = MinMaxAddi.MinimumValue;
		InitStatsValue();
	}

	public StatsDouble(MinMax MinMaxTotalAndFlat)
	{
		MaxTotal = MinMaxTotalAndFlat.MaximumValue;
		MinTotal = MinMaxTotalAndFlat.MinimumValue;
		MaxFlat = MinMaxTotalAndFlat.MaximumValue;
		MinFlat = MinMaxTotalAndFlat.MinimumValue;
		MaxAddi = double.MaxValue;
		MinAddi = -90.0;
		InitStatsValue();
	}

	public StatsDouble CopyMe()
	{
		StatsDouble statsDouble = new StatsDouble();
		statsDouble.MinTotal = MinTotal;
		statsDouble.MaxTotal = MaxTotal;
		statsDouble.MinFlat = MinFlat;
		statsDouble.MaxFlat = MaxFlat;
		statsDouble.MinAddi = MinAddi;
		statsDouble.MaxAddi = MaxAddi;
		statsDouble.Total = Total.CopyMe();
		statsDouble.Flat = Flat.CopyMe();
		statsDouble.AdditiveIncreases = AdditiveIncreases.CopyMe();
		statsDouble.MultiplicativeIncreases = MultiplicativeIncreases.CopyMe();
		statsDouble.CalculateTotal();
		return statsDouble;
	}

	public void CalculateTotal()
	{
		if (Flat.RealValue < 1000.0 && Flat.RealValue > -1000.0)
		{
			Total.SetOnlyValue = Math.Round(Flat.RealValue * (1.0 + AdditiveIncreases.RealValue / 100.0) * MultiplicativeIncreases.RealValue, 2);
		}
		else
		{
			Total.SetOnlyValue = Math.Round(Flat.RealValue * (1.0 + AdditiveIncreases.RealValue / 100.0) * MultiplicativeIncreases.RealValue);
		}
	}

	public void ResetAllDataToZero()
	{
		Flat.SetOnlyValue = 0.0;
		AdditiveIncreases.SetOnlyValue = 0.0;
		MultiplicativeIncreases.SetOnlyValue = 1.0;
		CalculateTotal();
	}

	public override string ToString()
	{
		return Total.RealValue.ToReadable(isForceReadable: false, 2);
	}
}
[Serializable]
public class StatsBool
{
	public bool Value;

	public StatsBool()
	{
		Value = false;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
[Serializable]
public class MinMax
{
	public double MinimumValue;

	public double MaximumValue;

	public MinMax(double min, double max)
	{
		MinimumValue = min;
		MaximumValue = max;
	}
}
public enum StatsProperties
{
	Flat,
	Additive,
	Multiplicative,
	SetFlat_And_ResetAddMulti,
	SetFlatOnly,
	SetAdditiveOnly,
	SetMultiplicativeOnly
}
public class StatsIntToFloatConverter : JsonConverter<StatsFloat>
{
	public override bool CanWrite => false;

	public override StatsFloat ReadJson(JsonReader reader, Type objectType, StatsFloat existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null)
		{
			return null;
		}
		JObject jObject = JObject.Load(reader);
		if (IsStatsFloatFormat(jObject))
		{
			return jObject.ToObject<StatsFloat>(serializer);
		}
		StatsFloat statsFloat = new StatsFloat();
		if (jObject["MinTotal"] != null)
		{
			statsFloat.MinTotal = jObject["MinTotal"].Value<int>();
		}
		if (jObject["MaxTotal"] != null)
		{
			statsFloat.MaxTotal = jObject["MaxTotal"].Value<int>();
		}
		if (jObject["MinFlat"] != null)
		{
			statsFloat.MinFlat = jObject["MinFlat"].Value<int>();
		}
		if (jObject["MaxFlat"] != null)
		{
			statsFloat.MaxFlat = jObject["MaxFlat"].Value<int>();
		}
		if (jObject["MinAddi"] != null)
		{
			statsFloat.MinAddi = jObject["MinAddi"].Value<float>();
		}
		if (jObject["MaxAddi"] != null)
		{
			statsFloat.MaxAddi = jObject["MaxAddi"].Value<float>();
		}
		if (jObject["Total"] != null)
		{
			statsFloat.Total = ConvertStatsValueIntToFloat(jObject["Total"]);
		}
		if (jObject["Flat"] != null)
		{
			statsFloat.Flat = ConvertStatsValueIntToFloat(jObject["Flat"]);
		}
		if (jObject["AdditiveIncreases"] != null)
		{
			statsFloat.AdditiveIncreases = jObject["AdditiveIncreases"].ToObject<StatsValueFloat>(serializer);
		}
		if (jObject["MultiplicativeIncreases"] != null)
		{
			statsFloat.MultiplicativeIncreases = jObject["MultiplicativeIncreases"].ToObject<StatsValueFloat>(serializer);
		}
		statsFloat.CalculateTotal();
		return statsFloat;
	}

	public override void WriteJson(JsonWriter writer, StatsFloat value, JsonSerializer serializer)
	{
		throw new NotImplementedException("Writing is not supported by this converter.");
	}

	private bool IsStatsFloatFormat(JObject jsonObject)
	{
		JToken jToken = jsonObject["Total"];
		if (jToken != null && jToken["_Value"] != null)
		{
			return jToken["_Value"].Type == JTokenType.Float;
		}
		JToken jToken2 = jsonObject["Flat"];
		if (jToken2 != null && jToken2["_Value"] != null)
		{
			return jToken2["_Value"].Type == JTokenType.Float;
		}
		if (jsonObject["MinTotal"] != null)
		{
			return jsonObject["MinTotal"].Type == JTokenType.Float;
		}
		if (jsonObject["MaxTotal"] != null)
		{
			return jsonObject["MaxTotal"].Type == JTokenType.Float;
		}
		if (jsonObject["MinFlat"] != null)
		{
			return jsonObject["MinFlat"].Type == JTokenType.Float;
		}
		if (jsonObject["MaxFlat"] != null)
		{
			return jsonObject["MaxFlat"].Type == JTokenType.Float;
		}
		return true;
	}

	private StatsValueFloat ConvertStatsValueIntToFloat(JToken statsValueIntToken)
	{
		StatsValueFloat statsValueFloat = new StatsValueFloat();
		if (statsValueIntToken["_Value"] != null)
		{
			statsValueFloat._Value = statsValueIntToken["_Value"].Value<int>();
		}
		if (statsValueIntToken["RealValue"] != null)
		{
			statsValueFloat.RealValue = statsValueIntToken["RealValue"].Value<int>();
		}
		if (statsValueIntToken["Max"] != null)
		{
			statsValueFloat.Max = statsValueIntToken["Max"].Value<int>();
		}
		if (statsValueIntToken["Min"] != null)
		{
			statsValueFloat.Min = statsValueIntToken["Min"].Value<int>();
		}
		return statsValueFloat;
	}
}
public class TowerCircleProjectileSelfer : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}
}
[CreateAssetMenu]
public class TowerInfo : ScriptableObject
{
	[HideInInspector]
	public string functionName;

	public ProjectileInfo projectileInfo;

	public float chance = 100f;

	public float AttacksPerSecond = 1f;
}
public class TowerSelfer : MonoBehaviour
{
	public TowerInfo myInfo;

	private bool isInit;

	private SpriteRenderer towerRenderer;

	private Transform FirePoint;

	private Transform MyGFXTrans;

	private float AttackTimer;

	private Color originalColor;

	private Vector3 originalScale;

	private int ProjectilesFired;

	[HideInInspector]
	public bool isBuilt;

	private GameObject BuildCanvas;

	private GameObject HighlightGO;

	public void TakeInfo()
	{
		if (!isInit)
		{
			isInit = true;
			towerRenderer = base.transform.Find("GFX").GetComponent<SpriteRenderer>();
			FirePoint = base.transform.Find("FirePoint");
			MyGFXTrans = base.transform.Find("GFX");
			originalColor = towerRenderer.color;
			originalScale = MyGFXTrans.localScale;
			BuildCanvas = FirePoint.Find("BuildCanvas").gameObject;
			HighlightGO = BuildCanvas.transform.Find("BuildTowerPrefab").Find("Highlight").gameObject;
		}
		AttackTimer = Mathf.Clamp(1f / (myInfo.AttacksPerSecond * playerData.instance.stats.TowersAttackSpeedMultiplier.Total.RealValue) - 0.5f, 0f, 100f);
		ProjectilesFired = 0;
		towerRenderer.sortingOrder = Mathf.RoundToInt((0f - base.transform.position.y) * 10f);
		towerRenderer.color = new Color(towerRenderer.color.r, towerRenderer.color.g, towerRenderer.color.b, 0.5f);
		BuildCanvas.SetActive(value: true);
		HighlightGO.SetActive(value: false);
		isBuilt = false;
		if (playerData.instance.stats.TowersSpawnAutomatically.Total.RealValue > 0)
		{
			BuildTower();
		}
	}

	public void BuildTower()
	{
		isBuilt = true;
		towerRenderer.color = originalColor;
		BuildCanvas.SetActive(value: false);
		TowersManager.instance.CheckIfTowerIsBuilt_ForAutomatorBot();
		FXManager.instance.PlayUIClickSound();
		FXManager.instance.SpawnGFX("BuildTowerFX", FirePoint.position, 3f, Vector3.one, 0f, ForceShow: true);
		FXManager.instance.PlaySound("BuildTowerSound", ForcePlay: true);
	}

	private void AttackFunction()
	{
		MyGFXTrans.DOKill(complete: true);
		towerRenderer.DOKill(complete: true);
		DG.Tweening.Sequence s = DOTween.Sequence();
		s.Append(MyGFXTrans.DOScale(originalScale * 0.93f, 0.08f).SetEase(Ease.InQuad)).Join(towerRenderer.DOColor(new Color(1f, 1f, 1.2f, 1f), 0.08f));
		s.Append(MyGFXTrans.DOScale(originalScale * 1.05f, 0.12f).SetEase(Ease.OutBack)).Join(MyGFXTrans.DOPunchRotation(new Vector3(0f, 0f, UnityEngine.Random.Range(-6f, 6f)), 0.12f, 8, 0.5f)).Join(towerRenderer.DOColor(new Color(1.3f, 1.1f, 0.9f, 1f), 0.06f));
		s.JoinCallback(delegate
		{
			Shoot();
		});
		s.Append(MyGFXTrans.DOScale(originalScale, 0.15f).SetEase(Ease.OutElastic, 0.8f, 0.5f)).Join(towerRenderer.DOColor(originalColor, 0.15f).SetEase(Ease.OutQuad));
	}

	private void Shoot()
	{
		if (myInfo.functionName == "TowerAoE")
		{
			if (FXManager.instance.SpawnGFX("FiringDust", FirePoint.position, 3.5f, Vector3.one * 27f) != null)
			{
				FXManager.instance.PlaySound("TowerFireSound", ForcePlay: true);
			}
			EnemySelfer farthestEnemy = EnemiesManager.instance.GetFarthestEnemy(FirePoint.position);
			if (farthestEnemy != null)
			{
				ProjectilesManager.instance.FireProjectile(FirePoint.position, farthestEnemy.GetPosition(), myInfo.projectileInfo, MultipleProjectileFormation.Circular, playerData.instance.stats.TowerAoE_NumberOfProjectiles.Total.RealValue, 25f, isFiredFromTowerDirectly: true);
			}
			CreateExpandingRing();
		}
		else if (myInfo.functionName == "TowerCircle")
		{
			if (FXManager.instance.SpawnGFX("FiringDust", FirePoint.position, 3.5f, Vector3.one * 27f) != null)
			{
				FXManager.instance.PlaySound("TowerFireSound", ForcePlay: true);
			}
			float num = 200f;
			int realValue = playerData.instance.stats.NumberOfProjectilesOfCircle.Total.RealValue;
			for (int i = 0; i < realValue; i++)
			{
				float f = 360f / (float)realValue * (float)i * (MathF.PI / 180f);
				Vector3 vector = new Vector3(Mathf.Cos(f) * num, Mathf.Sin(f) * num, 0f);
				ProjectilesManager.instance.FireProjectile(FirePoint.position + vector, FirePoint.position, myInfo.projectileInfo, MultipleProjectileFormation.GMP, 1, 0f, isFiredFromTowerDirectly: true);
				ProjectilesFired++;
			}
			CreateCircularBurst();
		}
		else if (myInfo.functionName == "TowerGold")
		{
			double num2 = DatabaseManager.EnemyGold(playerData.instance.MonstersLevel) * (double)playerData.instance.stats.TowerGold_GoldMultiplier.Total.RealValue;
			num2 *= (double)playerData.instance.stats.GoldGained.Total.RealValue;
			num2 = Math.Ceiling(num2);
			string text = num2.ToReadable();
			if (RunManager.instance.isDoubleGoldRun && RunManager.instance.isDoubleLootRun)
			{
				num2 *= 4.0;
			}
			else if (RunManager.instance.isDoubleGoldRun || RunManager.instance.isDoubleLootRun)
			{
				num2 *= 2.0;
			}
			FloatingNumbersManager.instance.GenerateFloatingNumber("<sprite name=Gold>" + text, FirePoint.position, "GoldFloater");
			playerData.instance.TotalCurrenciesGained_CurrentRun[Currencies.Gold] += num2;
			playerData.instance.TotalCurrenciesGained_FullGame[Currencies.Gold] += num2;
			PlayerManager.instance.ChangeCurrency(Currencies.Gold, num2, isApplyGainMulti: false);
			FXManager.instance.PlayGeneralSound(GeneralSounds.gold_gain_one);
			CreateGoldSparkle();
		}
		else if (myInfo.functionName == "TowerPierce")
		{
			if (FXManager.instance.SpawnGFX("FiringDust", FirePoint.position, 3.5f, Vector3.one * 27f) != null)
			{
				FXManager.instance.PlaySound("TowerFireSound", ForcePlay: true);
			}
			CreatePiercingBeam();
			EnemySelfer farthestEnemy2 = EnemiesManager.instance.GetFarthestEnemy(FirePoint.position);
			if (farthestEnemy2 != null)
			{
				float num3 = FunctionsNeeded.CalculateAngle(farthestEnemy2.GetPosition() - (Vector2)FirePoint.position, IsRadian: true);
				Vector2 vector2 = new Vector2(Mathf.Cos(num3), Mathf.Sin(num3));
				ProjectilesManager.instance.FireProjectile(FirePoint.position, FirePoint.position + (Vector3)vector2 * 1000f, myInfo.projectileInfo, MultipleProjectileFormation.GMP, 1, 25f, isFiredFromTowerDirectly: true);
				if (FunctionsNeeded.IsHappened(playerData.instance.stats.TowerPierce_ChanceToFireBehind.Total.RealValue))
				{
					vector2 = new Vector2(Mathf.Cos(num3 + MathF.PI), Mathf.Sin(num3 + MathF.PI));
					ProjectilesManager.instance.FireProjectile(FirePoint.position, FirePoint.position + (Vector3)vector2 * 1000f, myInfo.projectileInfo, MultipleProjectileFormation.GMP, 1, 25f, isFiredFromTowerDirectly: true);
				}
			}
		}
		else if (myInfo.functionName == "TowerQuick")
		{
			if (FXManager.instance.SpawnGFX("FiringDust", FirePoint.position, 3.5f, Vector3.one * 27f) != null)
			{
				FXManager.instance.PlaySound("TowerFireSound", ForcePlay: true);
			}
			EnemySelfer nearestEnemy = EnemiesManager.instance.GetNearestEnemy(FirePoint.position, 6000f);
			if (nearestEnemy != null)
			{
				ProjectilesManager.instance.FireProjectile(FirePoint.position, nearestEnemy.GetPosition(), myInfo.projectileInfo, MultipleProjectileFormation.GMP, 1, 25f, isFiredFromTowerDirectly: true);
			}
		}
		ProjectilesFired++;
	}

	private void CreateExpandingRing()
	{
		GameObject ring = new GameObject("TempRing");
		ring.transform.position = FirePoint.position;
		SpriteRenderer spriteRenderer = ring.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = towerRenderer.sprite;
		spriteRenderer.color = new Color(towerRenderer.color.r, towerRenderer.color.g, towerRenderer.color.b, 0.5f);
		spriteRenderer.sortingOrder = towerRenderer.sortingOrder - 1;
		ring.transform.localScale = Vector3.one * 0.3f;
		ring.transform.DOScale(Vector3.one * 2f, 0.4f).SetEase(Ease.OutQuad);
		spriteRenderer.DOFade(0f, 0.4f).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(ring);
		});
	}

	private void CreateCircularBurst()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject ring = new GameObject("TempRing");
			ring.transform.position = FirePoint.position;
			SpriteRenderer spriteRenderer = ring.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = towerRenderer.sprite;
			spriteRenderer.color = new Color(towerRenderer.color.r, towerRenderer.color.g, towerRenderer.color.b, 0.4f);
			spriteRenderer.sortingOrder = towerRenderer.sortingOrder - 1;
			ring.transform.localScale = Vector3.one * 0.2f;
			float delay = (float)i * 0.1f;
			ring.transform.DOScale(Vector3.one * 1.8f, 0.35f).SetEase(Ease.OutQuad).SetDelay(delay);
			spriteRenderer.DOFade(0f, 0.35f).SetEase(Ease.OutQuad).SetDelay(delay)
				.OnComplete(delegate
				{
					UnityEngine.Object.Destroy(ring);
				});
		}
	}

	private void CreateGoldSparkle()
	{
		GameObject sparkle = new GameObject("TempSparkle");
		sparkle.transform.position = FirePoint.position;
		SpriteRenderer spriteRenderer = sparkle.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = towerRenderer.sprite;
		spriteRenderer.color = new Color(1f, 0.84f, 0f, 0.8f);
		spriteRenderer.sortingOrder = towerRenderer.sortingOrder + 1;
		sparkle.transform.localScale = Vector3.one * 0.5f;
		sparkle.transform.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutQuad);
		sparkle.transform.DORotate(new Vector3(0f, 0f, 360f), 0.3f, DG.Tweening.RotateMode.FastBeyond360);
		spriteRenderer.DOFade(0f, 0.3f).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(sparkle);
		});
	}

	private void CreatePiercingBeam()
	{
		GameObject beam = new GameObject("TempBeam");
		beam.transform.position = FirePoint.position;
		SpriteRenderer spriteRenderer = beam.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = towerRenderer.sprite;
		spriteRenderer.color = new Color(towerRenderer.color.r, towerRenderer.color.g, towerRenderer.color.b, 0.7f);
		spriteRenderer.sortingOrder = towerRenderer.sortingOrder;
		beam.transform.localScale = new Vector3(0.3f, 2f, 1f);
		beam.transform.DOScaleX(0.1f, 0.25f).SetEase(Ease.InQuad);
		spriteRenderer.DOFade(0f, 0.25f).SetEase(Ease.InQuad).OnComplete(delegate
		{
			UnityEngine.Object.Destroy(beam);
		});
	}

	private void Update()
	{
		if (isBuilt && (!(myInfo.functionName == "TowerCircle") || ProjectilesFired < playerData.instance.stats.NumberOfProjectilesOfCircle.Total.RealValue))
		{
			AttackTimer += Time.deltaTime;
			if (AttackTimer >= 1f / (myInfo.AttacksPerSecond * playerData.instance.stats.TowersAttackSpeedMultiplier.Total.RealValue))
			{
				AttackTimer = 0f;
				AttackFunction();
			}
		}
	}
}
public class TowersManager : MonoBehaviour
{
	public static TowersManager instance;

	private Dictionary<string, float> TowerChances = new Dictionary<string, float>();

	private List<TowerSelfer> AliveTowers = new List<TowerSelfer>();

	private float SpawnTimer;

	private Coroutine spawnTowersAtStartCoroutine;

	private Coroutine spawnTowersAtMiddleCoroutine;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		foreach (TowerInfo tower in DatabaseManager.TowerList)
		{
			TowerChances.Add(tower.functionName, tower.chance);
		}
	}

	public void StartRun()
	{
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Towers])
		{
			AliveTowers = new List<TowerSelfer>();
			SpawnTimer = 0f;
			TowerChances = new Dictionary<string, float>();
			if (playerData.instance.stats.TowerAoE_CanBeFoundInRun.Total.RealValue > 50)
			{
				TowerChances.Add("TowerAoE", DatabaseManager.TowerDict["TowerAoE"].chance);
			}
			if (playerData.instance.stats.TowerCircle_CanBeFoundInRun.Total.RealValue > 50)
			{
				TowerChances.Add("TowerCircle", DatabaseManager.TowerDict["TowerCircle"].chance);
			}
			if (playerData.instance.stats.TowerGold_CanBeFoundInRun.Total.RealValue > 50)
			{
				TowerChances.Add("TowerGold", DatabaseManager.TowerDict["TowerGold"].chance);
			}
			if (playerData.instance.stats.TowerPierce_CanBeFoundInRun.Total.RealValue > 50)
			{
				TowerChances.Add("TowerPierce", DatabaseManager.TowerDict["TowerPierce"].chance);
			}
			if (playerData.instance.stats.TowerQuick_CanBeFoundInRun.Total.RealValue > 50)
			{
				TowerChances.Add("TowerQuick", DatabaseManager.TowerDict["TowerQuick"].chance);
			}
			spawnTowersAtStartCoroutine = StartCoroutine(SpawnTowersAtStartDelayed());
			spawnTowersAtMiddleCoroutine = StartCoroutine(SpawnTowersAtMiddleDelayed());
		}
	}

	private IEnumerator SpawnTowersAtStartDelayed()
	{
		for (int i = 0; i < playerData.instance.stats.NumbersOfTowersSpawnAtRunStart.Total.RealValue; i++)
		{
			CreateARandomTower();
			yield return new WaitForSeconds(0.25f);
		}
		CheckIfTowerIsBuilt_ForAutomatorBot();
	}

	private IEnumerator SpawnTowersAtMiddleDelayed()
	{
		yield return new WaitForSeconds(playerData.instance.stats.Timer.Total.RealValue / 2f);
		if (playerData.instance.stats.Well_SpawnAllTowersTypesAtRunStart.Total.RealValue > 50)
		{
			for (int i = 0; i < DatabaseManager.TowerList.Count; i++)
			{
				CreateARandomTower(DatabaseManager.TowerList[i].functionName);
				yield return new WaitForSeconds(0.25f);
			}
		}
	}

	public void CheckIfTowerIsBuilt_ForAutomatorBot()
	{
		foreach (TowerSelfer aliveTower in AliveTowers)
		{
			if (!aliveTower.isBuilt)
			{
				AutomatorBot.instance.TowerFound(isTaken: false, aliveTower.transform.position);
				return;
			}
		}
		AutomatorBot.instance.TowerFound(isTaken: true, Vector2.zero);
	}

	public void EndRun()
	{
		if (spawnTowersAtStartCoroutine != null)
		{
			StopCoroutine(spawnTowersAtStartCoroutine);
			spawnTowersAtStartCoroutine = null;
		}
		if (spawnTowersAtMiddleCoroutine != null)
		{
			StopCoroutine(spawnTowersAtMiddleCoroutine);
			spawnTowersAtMiddleCoroutine = null;
		}
		DestroyAllTowers();
	}

	private Vector2 GetRandomPosition()
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < 10; i++)
		{
			list.Add(EnemiesManager.instance.SampleRandomPointInTrapezoid(isAvoidEnemies: true));
		}
		if (AliveTowers.Count > 0)
		{
			float num = -1f;
			Vector2 result = list[0];
			{
				foreach (Vector2 item in list)
				{
					float num2 = float.MaxValue;
					foreach (TowerSelfer aliveTower in AliveTowers)
					{
						float num3 = Vector2.Distance(item, aliveTower.transform.position);
						if (num3 < num2)
						{
							num2 = num3;
						}
					}
					if (num2 > num)
					{
						num = num2;
						result = item;
					}
				}
				return result;
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public void CreateARandomTower(string towerName = "")
	{
		string whichPool = ((towerName == "") ? FunctionsNeeded.GetARandomFromDict_Normal(TowerChances) : towerName);
		if (playerData.instance.isJustUnlockedAoETower)
		{
			whichPool = "TowerAoE";
			playerData.instance.isJustUnlockedAoETower = false;
		}
		if (playerData.instance.isJustUnlockedGoldTower)
		{
			whichPool = "TowerGold";
			playerData.instance.isJustUnlockedGoldTower = false;
		}
		if (playerData.instance.isJustUnlockedPierceTower)
		{
			whichPool = "TowerPierce";
			playerData.instance.isJustUnlockedPierceTower = false;
		}
		if (playerData.instance.isJustUnlockedCircleTower)
		{
			whichPool = "TowerCircle";
			playerData.instance.isJustUnlockedCircleTower = false;
		}
		TowerSelfer component = ObjectPooler.instance.GiveMeObject(whichPool, base.transform, GetRandomPosition()).GetComponent<TowerSelfer>();
		component.TakeInfo();
		AliveTowers.Add(component);
		FXManager.instance.PlaySound("SpawnTowerSound");
	}

	public void DestroyAllTowers()
	{
		for (int num = AliveTowers.Count - 1; num >= 0; num--)
		{
			ObjectPooler.instance.ReturnObjectToPool(AliveTowers[num].gameObject, AliveTowers[num].myInfo.functionName);
			AliveTowers.RemoveAt(num);
		}
	}

	private void Update()
	{
		if (RunManager.instance.isRunStarted && playerData.instance.UnlockedSystems[UnlockableSystems.Towers] && playerData.instance.stats.TowerSpawnTime.Total.RealValue > 2f)
		{
			SpawnTimer += Time.deltaTime;
			if (SpawnTimer >= playerData.instance.stats.TowerSpawnTime.Total.RealValue)
			{
				SpawnTimer = 0f;
				CreateARandomTower();
			}
		}
	}
}
public static class ExpressionEvaluator
{
	private class Parser
	{
		private readonly string _s;

		private int _pos;

		private readonly double _x;

		public Parser(string s, double x)
		{
			_s = s;
			_pos = 0;
			_x = x;
		}

		public double Parse()
		{
			double result = ParseExpression();
			SkipWhitespace();
			if (_pos < _s.Length)
			{
				throw new Exception($"Unexpected character at position {_pos}: '{_s[_pos]}'");
			}
			return result;
		}

		private double ParseExpression()
		{
			double num = ParseTerm();
			while (true)
			{
				SkipWhitespace();
				if (_pos < _s.Length && _s[_pos] == '+')
				{
					_pos++;
					num += ParseTerm();
					continue;
				}
				if (_pos >= _s.Length || _s[_pos] != '-')
				{
					break;
				}
				_pos++;
				num -= ParseTerm();
			}
			return num;
		}

		private double ParseTerm()
		{
			double num = ParseFactor();
			while (true)
			{
				SkipWhitespace();
				if (_pos < _s.Length && _s[_pos] == '*')
				{
					_pos++;
					num *= ParseFactor();
					continue;
				}
				if (_pos >= _s.Length || _s[_pos] != '/')
				{
					break;
				}
				_pos++;
				num /= ParseFactor();
			}
			return num;
		}

		private double ParseFactor()
		{
			double num = ParsePrimary();
			SkipWhitespace();
			if (_pos < _s.Length && _s[_pos] == '^')
			{
				_pos++;
				double y = ParseFactor();
				num = Math.Pow(num, y);
			}
			return num;
		}

		private double ParsePrimary()
		{
			SkipWhitespace();
			if (_pos >= _s.Length)
			{
				throw new Exception("Unexpected end of input");
			}
			bool flag = false;
			if (_s[_pos] == '-')
			{
				flag = true;
				_pos++;
				SkipWhitespace();
				if (_pos >= _s.Length)
				{
					throw new Exception("Unexpected end of input after unary minus");
				}
			}
			char c = _s[_pos];
			double num;
			if (c == '(')
			{
				_pos++;
				num = ParseExpression();
				SkipWhitespace();
				if (_pos >= _s.Length || _s[_pos] != ')')
				{
					throw new Exception("Missing closing parenthesis");
				}
				_pos++;
			}
			else if (char.IsLetter(c))
			{
				string text = ParseName();
				if (!text.Equals("x", StringComparison.OrdinalIgnoreCase) && !text.Equals("value", StringComparison.OrdinalIgnoreCase))
				{
					throw new Exception("Unknown variable '" + text + "'");
				}
				num = _x;
			}
			else
			{
				num = ParseNumber();
			}
			if (!flag)
			{
				return num;
			}
			return 0.0 - num;
		}

		private void SkipWhitespace()
		{
			while (_pos < _s.Length && char.IsWhiteSpace(_s[_pos]))
			{
				_pos++;
			}
		}

		private string ParseName()
		{
			int pos = _pos;
			while (_pos < _s.Length && char.IsLetter(_s[_pos]))
			{
				_pos++;
			}
			return _s.Substring(pos, _pos - pos);
		}

		private double ParseNumber()
		{
			int pos = _pos;
			bool flag = false;
			bool flag2 = false;
			while (_pos < _s.Length)
			{
				char c = _s[_pos];
				if (char.IsDigit(c))
				{
					_pos++;
					continue;
				}
				if ((c == '.' || c == ',') && !flag && !flag2)
				{
					flag = true;
					_pos++;
					continue;
				}
				if ((c != 'e' && c != 'E') || flag2)
				{
					break;
				}
				flag2 = true;
				_pos++;
				if (_pos < _s.Length && (_s[_pos] == '+' || _s[_pos] == '-'))
				{
					_pos++;
				}
				if (_pos >= _s.Length || !char.IsDigit(_s[_pos]))
				{
					throw new Exception("Invalid scientific notation: expected digit after 'e'");
				}
				while (_pos < _s.Length && char.IsDigit(_s[_pos]))
				{
					_pos++;
				}
				break;
			}
			string text = _s.Substring(pos, _pos - pos);
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new Exception("Invalid number: empty token");
			}
			if (!double.TryParse(text.Replace(',', '.'), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
			{
				throw new Exception("Invalid number '" + text + "'");
			}
			return result;
		}
	}

	public static double Evaluate(string equation, double value)
	{
		return new Parser(equation, value).Parse();
	}
}
[RequireComponent(typeof(TreeState))]
[ExecuteInEditMode]
public class TreeCreator : MonoBehaviour
{
	[Serializable]
	private class NodeReference
	{
		public TreeNodeInfo nodeInfo;

		public GameObject gameObject;
	}

	[Header("Tree Configuration")]
	[SerializeField]
	private TreeInfo treeAsset;

	[SerializeField]
	private GameObject nodePrefab;

	[SerializeField]
	private GameObject linkPrefab;

	public TooltipController tooltip;

	public Sprite DefaultNodeShape_Normal;

	public Sprite FullyGainedNodeShape_Normal;

	public Sprite DefaultNodeShape_Boss;

	public Sprite FullyGainedNodeShape_Boss;

	[Header("Runtime References")]
	[SerializeField]
	private Transform nodesParent;

	[SerializeField]
	private Transform linksParent;

	[Header("Tree Settings")]
	[SerializeField]
	private float distancesMultiplier = 1f;

	[SerializeField]
	private bool enableRealtimeSync = true;

	[SerializeField]
	private bool centerTree = true;

	[SerializeField]
	private InaccessibleNodeDisplayMode inaccessibleNodeDisplay = InaccessibleNodeDisplayMode.Show;

	[Header("Link Visual Settings")]
	[SerializeField]
	private Sprite defaultLinkSprite;

	[SerializeField]
	private Color defaultUnlockedLinkColor = Color.white;

	[SerializeField]
	private Color defaultLockedLinkColor = Color.gray;

	private float syncDelay;

	private Dictionary<TreeNodeInfo, GameObject> instantiatedNodes = new Dictionary<TreeNodeInfo, GameObject>();

	private Dictionary<TreeNodeInfo, Image> instantiatedNodes_Shapes = new Dictionary<TreeNodeInfo, Image>();

	private Dictionary<TreeNodeInfo, Image> instantiatedNodes_Icons = new Dictionary<TreeNodeInfo, Image>();

	[SerializeField]
	[HideInInspector]
	private List<NodeReference> serializedNodeReferences = new List<NodeReference>();

	private TreeInfo lastTrackedTree;

	private bool pendingSyncUpdate;

	private float lastSyncTime;

	private bool isSyncing;

	private bool hasRestoredFromSerialization;

	private TreeState treeState;

	private Dictionary<(GameObject, GameObject), UILineRenderer> createdLinks_gameObjects = new Dictionary<(GameObject, GameObject), UILineRenderer>();

	private Dictionary<(TreeNodeInfo, TreeNodeInfo), UILineRenderer> createdLinks_infos = new Dictionary<(TreeNodeInfo, TreeNodeInfo), UILineRenderer>();

	public bool EnableRealtimeSync
	{
		get
		{
			return enableRealtimeSync;
		}
		set
		{
			if (enableRealtimeSync != value)
			{
				enableRealtimeSync = value;
				if (enableRealtimeSync)
				{
					SubscribeToTreeEvents();
					ScheduleSyncUpdate();
				}
				else
				{
					UnsubscribeFromTreeEvents();
				}
			}
		}
	}

	public float SyncDelay
	{
		get
		{
			return syncDelay;
		}
		set
		{
			syncDelay = Mathf.Max(0.01f, value);
		}
	}

	public bool AutoCenterTree
	{
		get
		{
			return centerTree;
		}
		set
		{
			centerTree = value;
		}
	}

	public TreeInfo TreeAsset => treeAsset;

	public GameObject NodePrefab => nodePrefab;

	public GameObject LinkPrefab => linkPrefab;

	public float DistancesMultiplier => distancesMultiplier;

	public Sprite DefaultLinkSprite => defaultLinkSprite;

	public Color DefaultUnlockedLinkColor => defaultUnlockedLinkColor;

	public Color DefaultLockedLinkColor => defaultLockedLinkColor;

	private void ApplyCentering()
	{
		if (!centerTree || nodesParent == null || nodesParent.childCount == 0)
		{
			return;
		}
		Vector3 vector = Vector3.positiveInfinity;
		Vector3 vector2 = Vector3.negativeInfinity;
		for (int i = 0; i < nodesParent.childCount; i++)
		{
			Vector3 localPosition = nodesParent.GetChild(i).localPosition;
			vector = Vector3.Min(vector, localPosition);
			vector2 = Vector3.Max(vector2, localPosition);
		}
		Vector3 vector3 = (vector + vector2) * 0.5f;
		Vector3 vector4 = Vector3.zero - vector3;
		if (vector4.magnitude > 0.01f)
		{
			for (int j = 0; j < nodesParent.childCount; j++)
			{
				nodesParent.GetChild(j).localPosition += vector4;
			}
			if (linksParent != null && instantiatedNodes.Count > 0)
			{
				RecreateAllLinks();
			}
		}
	}

	private void ApplyUncentering()
	{
		if (treeAsset == null || nodesParent == null || nodesParent.childCount == 0)
		{
			return;
		}
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			TreeNodeInfo key = instantiatedNode.Key;
			GameObject value = instantiatedNode.Value;
			if (key != null && value != null)
			{
				Vector2 vector = CalculateNodeWorldPosition(key);
				value.transform.localPosition = vector;
			}
		}
		if (linksParent != null && instantiatedNodes.Count > 0)
		{
			RecreateAllLinks();
		}
	}

	private void DebugLog(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	private void DebugLogWarning(string message)
	{
		UnityEngine.Debug.LogWarning(message);
	}

	private void DebugLogError(string message)
	{
		UnityEngine.Debug.LogError(message);
	}

	private void OnEnable()
	{
		if (!hasRestoredFromSerialization)
		{
			RestoreInstantiatedNodesFromSerialization();
			hasRestoredFromSerialization = true;
		}
		if (!enableRealtimeSync)
		{
			return;
		}
		SubscribeToTreeEvents();
		if (treeAsset != null)
		{
			if (ShouldPerformFullSync())
			{
				SyncWithTreeData();
			}
			else
			{
				TriggerLineRecovery();
			}
		}
	}

	private void OnDisable()
	{
		if (instantiatedNodes.Count > 0)
		{
			SaveInstantiatedNodesToSerialization();
		}
		hasRestoredFromSerialization = false;
		UnsubscribeFromTreeEvents();
		if (enableRealtimeSync && linksParent != null)
		{
			CleanupUILineRendererSegments();
		}
	}

	private void EditorUpdate()
	{
		if (!Application.isPlaying && this != null && base.enabled)
		{
			HandleUpdateLogic();
		}
	}

	private void HandleUpdateLogic()
	{
		if (enableRealtimeSync && lastTrackedTree != treeAsset)
		{
			DebugLog("[TreeCreator] Asset changed during update, resubscribing to events");
			SubscribeToTreeEvents();
		}
		if (enableRealtimeSync && pendingSyncUpdate && Time.time - lastSyncTime > syncDelay)
		{
			HandleRealtimeSync();
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			StartTreeSystem();
		}
	}

	public void StartTreeSystem()
	{
		InitLinksDicts();
		treeState = GetComponent<TreeState>();
		treeState.StartTreeSystem();
		ApplyAccessibilityDisplayRules();
		UpdateAllNodeColors();
		UpdateAllLinkColors();
		Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.Gold] = (Action)Delegate.Combine(onCurrencyChange[Currencies.Gold], new Action(UpdateAllNodeShapeColors));
		onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
		onCurrencyChange[Currencies.ClearCurrency] = (Action)Delegate.Combine(onCurrencyChange[Currencies.ClearCurrency], new Action(UpdateAllNodeShapeColors));
		AzrarManager instance = AzrarManager.instance;
		instance.OnAzrarUnlocked = (Action)Delegate.Combine(instance.OnAzrarUnlocked, new Action(UpdateAllNodeShapeColors));
	}

	[ContextMenu("Create Tree")]
	public void CreateTree()
	{
		if (ValidateConfiguration())
		{
			CleanupOrphanedLineSegments();
			ClearExistingTree();
			CreateParentObjects();
			CreateNodes();
			CreateLinks();
			if (centerTree)
			{
				ApplyCentering();
			}
		}
	}

	[ContextMenu("Clear Tree")]
	public void ClearTree()
	{
		ClearExistingTree();
	}

	[ContextMenu("Manual Sync")]
	public void ManualSync()
	{
		SyncWithTreeData();
	}

	public void ForceImmediateSync()
	{
		if (treeAsset != null)
		{
			pendingSyncUpdate = false;
			SyncWithTreeData();
		}
	}

	[ContextMenu("Manual Center Tree")]
	public void CenterTree()
	{
		if (nodesParent == null || nodesParent.childCount == 0)
		{
			DebugLogWarning("TreeCreator: No nodes found to center.");
			return;
		}
		ApplyCentering();
		if (linksParent != null && instantiatedNodes.Count > 0)
		{
			RecreateAllLinks();
		}
	}

	[ContextMenu("Manual Uncenter Tree")]
	public void UncenterTree()
	{
		if (nodesParent == null || nodesParent.childCount == 0)
		{
			DebugLogWarning("TreeCreator: No nodes found to uncenter.");
		}
		else
		{
			ApplyUncentering();
		}
	}

	private bool ValidateConfiguration()
	{
		if (treeAsset == null)
		{
			DebugLogError("TreeCreator: Tree Asset is not assigned!");
			return false;
		}
		if (nodePrefab == null)
		{
			DebugLogError("TreeCreator: Node Prefab is not assigned!");
			return false;
		}
		if (linkPrefab == null)
		{
			DebugLogError("TreeCreator: Link Prefab is not assigned!");
			return false;
		}
		Transform transform = nodePrefab.transform.Find("NodeShape");
		Transform transform2 = nodePrefab.transform.Find("NodeIcon");
		if (transform == null)
		{
			DebugLogError("TreeCreator: Node Prefab must have a child named 'NodeShape'!");
			return false;
		}
		if (transform2 == null)
		{
			DebugLogError("TreeCreator: Node Prefab must have a child named 'NodeIcon'!");
			return false;
		}
		if (transform.GetComponent<Image>() == null)
		{
			DebugLogError("TreeCreator: 'NodeShape' child must have an Image component!");
			return false;
		}
		if (transform2.GetComponent<Image>() == null)
		{
			DebugLogError("TreeCreator: 'NodeIcon' child must have an Image component!");
			return false;
		}
		return true;
	}

	private void DestroyGameObject(GameObject obj)
	{
		if (obj != null)
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	private void CreateParentObjects()
	{
		if (linksParent == null)
		{
			GameObject gameObject = new GameObject("LinksParent");
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			linksParent = gameObject.transform;
		}
		if (nodesParent == null)
		{
			GameObject gameObject2 = new GameObject("NodesParent");
			gameObject2.transform.SetParent(base.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			nodesParent = gameObject2.transform;
		}
	}

	private Vector2 CalculateNodeWorldPosition(TreeNodeInfo node)
	{
		Vector2 vector = new Vector2(node.Position.x, node.Position.y);
		return treeAsset.FirstNodePosition + vector * distancesMultiplier;
	}

	public void ApplyAccessibilityDisplayRules()
	{
		if (inaccessibleNodeDisplay == InaccessibleNodeDisplayMode.Show)
		{
			ShowAllNodesAndConnections();
		}
		else if (inaccessibleNodeDisplay == InaccessibleNodeDisplayMode.Hide)
		{
			HideInaccessibleNodesAndConnections();
		}
	}

	private void ShowAllNodesAndConnections()
	{
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (instantiatedNode.Value != null)
			{
				instantiatedNode.Value.SetActive(value: true);
			}
		}
		if (!(linksParent != null))
		{
			return;
		}
		foreach (Transform item in linksParent)
		{
			if (item != null)
			{
				item.gameObject.SetActive(value: true);
			}
		}
	}

	private void HideInaccessibleNodesAndConnections()
	{
		if (treeAsset == null || treeAsset.TreeNodes == null)
		{
			return;
		}
		List<TreeNodeInfo> accessibleNodes = treeState.GetAccessibleNodes();
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (instantiatedNode.Key != null && instantiatedNode.Value != null)
			{
				bool active = accessibleNodes.Contains(instantiatedNode.Key);
				instantiatedNode.Value.SetActive(active);
			}
		}
		if (!(linksParent != null))
		{
			return;
		}
		foreach (Transform item in linksParent)
		{
			TreeLink component = item.GetComponent<TreeLink>();
			if (component != null)
			{
				TreeNodeInfo nodeInfoFromGameObject = GetNodeInfoFromGameObject(component.FromNode);
				TreeNodeInfo nodeInfoFromGameObject2 = GetNodeInfoFromGameObject(component.ToNode);
				bool active2 = nodeInfoFromGameObject != null && accessibleNodes.Contains(nodeInfoFromGameObject) && nodeInfoFromGameObject2 != null && accessibleNodes.Contains(nodeInfoFromGameObject2);
				item.gameObject.SetActive(active2);
			}
		}
	}

	public void ShowConnectedNodesAndLinks_OnNodeLevelChanged(TreeNodeInfo node)
	{
		if (inaccessibleNodeDisplay != 0)
		{
			return;
		}
		foreach (TreeNodeInfo allConnectedNode in node.GetAllConnectedNodes())
		{
			if (allConnectedNode != null && instantiatedNodes.ContainsKey(allConnectedNode))
			{
				instantiatedNodes[allConnectedNode].SetActive(value: true);
				ShowHideLink(node, allConnectedNode, show: true);
			}
		}
	}

	private TreeNodeInfo GetNodeInfoFromGameObject(GameObject nodeGameObject)
	{
		if (nodeGameObject == null)
		{
			return null;
		}
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (instantiatedNode.Value == nodeGameObject)
			{
				return instantiatedNode.Key;
			}
		}
		return null;
	}

	private void SaveInstantiatedNodesToSerialization()
	{
		serializedNodeReferences.Clear();
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (instantiatedNode.Key != null && instantiatedNode.Value != null)
			{
				serializedNodeReferences.Add(new NodeReference
				{
					nodeInfo = instantiatedNode.Key,
					gameObject = instantiatedNode.Value
				});
			}
		}
	}

	private void RestoreInstantiatedNodesFromSerialization()
	{
		instantiatedNodes.Clear();
		instantiatedNodes_Shapes.Clear();
		instantiatedNodes_Icons.Clear();
		int num = 0;
		int num2 = 0;
		foreach (NodeReference serializedNodeReference in serializedNodeReferences)
		{
			if (serializedNodeReference.nodeInfo != null && serializedNodeReference.gameObject != null)
			{
				instantiatedNodes[serializedNodeReference.nodeInfo] = serializedNodeReference.gameObject;
				Transform transform = serializedNodeReference.gameObject.transform.Find("NodeShape");
				Transform transform2 = serializedNodeReference.gameObject.transform.Find("NodeIcon");
				if (transform != null)
				{
					Image component = transform.GetComponent<Image>();
					if (component != null)
					{
						instantiatedNodes_Shapes[serializedNodeReference.nodeInfo] = component;
					}
				}
				if (transform2 != null)
				{
					Image component2 = transform2.GetComponent<Image>();
					if (component2 != null)
					{
						instantiatedNodes_Icons[serializedNodeReference.nodeInfo] = component2;
					}
				}
				num++;
			}
			else
			{
				num2++;
			}
		}
		if (serializedNodeReferences.Count == 0)
		{
			DiscoverExistingNodesInScene();
		}
	}

	private void DiscoverExistingNodesInScene()
	{
		if (treeAsset == null || treeAsset.TreeNodes == null || nodesParent == null)
		{
			return;
		}
		int num = 0;
		foreach (TreeNodeInfo treeNode in treeAsset.TreeNodes)
		{
			if (treeNode == null)
			{
				continue;
			}
			for (int i = 0; i < nodesParent.childCount; i++)
			{
				GameObject gameObject = nodesParent.GetChild(i).gameObject;
				if (!(gameObject.name == treeNode.NodeName))
				{
					continue;
				}
				instantiatedNodes[treeNode] = gameObject;
				Transform transform = gameObject.transform.Find("NodeShape");
				Transform transform2 = gameObject.transform.Find("NodeIcon");
				if (transform != null)
				{
					Image component = transform.GetComponent<Image>();
					if (component != null)
					{
						instantiatedNodes_Shapes[treeNode] = component;
					}
				}
				if (transform2 != null)
				{
					Image component2 = transform2.GetComponent<Image>();
					if (component2 != null)
					{
						instantiatedNodes_Icons[treeNode] = component2;
					}
				}
				num++;
				break;
			}
		}
		if (num > 0)
		{
			SaveInstantiatedNodesToSerialization();
		}
	}

	private void OnTreeChanged(TreeInfo tree)
	{
		if (tree == treeAsset)
		{
			ScheduleSyncUpdate();
		}
	}

	private void OnNodeAdded(TreeInfo tree, TreeNodeInfo node)
	{
		if (tree == treeAsset)
		{
			ScheduleSyncUpdate();
		}
	}

	private void OnNodeRemoved(TreeInfo tree, TreeNodeInfo node)
	{
		if (tree == treeAsset)
		{
			ScheduleSyncUpdate();
		}
	}

	private void OnTreeStructureChanged(TreeInfo tree)
	{
		if (tree == treeAsset)
		{
			ScheduleSyncUpdate();
		}
	}

	private void OnNodePropertyChanged(TreeNodeInfo node)
	{
		if (treeAsset != null && treeAsset.TreeNodes != null && treeAsset.TreeNodes.Contains(node))
		{
			UpdateSingleNodeVisuals(node);
		}
	}

	private void OnNodePositionChanged(TreeNodeInfo node)
	{
		if (treeAsset != null && treeAsset.TreeNodes != null && treeAsset.TreeNodes.Contains(node))
		{
			if (centerTree)
			{
				ScheduleSyncUpdate();
			}
			else
			{
				UpdateSingleNodePosition(node);
			}
		}
	}

	private void OnNodeConnectionsChanged(TreeNodeInfo node)
	{
		if (treeAsset != null && treeAsset.TreeNodes != null && treeAsset.TreeNodes.Contains(node))
		{
			ScheduleSyncUpdate();
		}
	}

	public void UpdateSingleNodeVisuals(TreeNodeInfo treeNode)
	{
		if (!(treeNode == null) && instantiatedNodes.ContainsKey(treeNode))
		{
			instantiatedNodes[treeNode].name = treeNode.NodeName;
			SetupNodeVisuals(treeNode);
		}
	}

	private void UpdateSingleNodePosition(TreeNodeInfo treeNode)
	{
		if (treeNode == null || !instantiatedNodes.ContainsKey(treeNode))
		{
			DebugLogWarning(string.Format("[TreeCreator] Cannot update node position - treeNode: {0}, inDict: {1}", (treeNode != null) ? treeNode.NodeName : "null", treeNode != null && instantiatedNodes.ContainsKey(treeNode)));
			return;
		}
		GameObject gameObject = instantiatedNodes[treeNode];
		if (gameObject == null)
		{
			DebugLogWarning("[TreeCreator] Node " + treeNode.NodeName + " GameObject is null, skipping position update");
			return;
		}
		Vector2 vector = CalculateNodeWorldPosition(treeNode);
		_ = gameObject.transform.localPosition;
		gameObject.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
		UpdateLinksForNode(treeNode);
	}

	public void UpdateLinksForNode(TreeNodeInfo treeNode)
	{
		if (treeNode == null || linksParent == null)
		{
			return;
		}
		UILineRenderer[] componentsInChildren = linksParent.GetComponentsInChildren<UILineRenderer>();
		int num = 0;
		UILineRenderer[] array = componentsInChildren;
		foreach (UILineRenderer uILineRenderer in array)
		{
			if (!(uILineRenderer != null))
			{
				continue;
			}
			string text = uILineRenderer.gameObject.name;
			if (text.Contains("Link_" + treeNode.NodeName + "_to_") || text.Contains("_to_" + treeNode.NodeName))
			{
				try
				{
					uILineRenderer.ForceSync();
					num++;
				}
				catch (Exception ex)
				{
					DebugLogError("[TreeCreator] Error updating link " + text + ": " + ex.Message);
				}
			}
		}
	}

	private void TriggerLineRecovery()
	{
		if (linksParent == null)
		{
			return;
		}
		UILineRenderer[] componentsInChildren = linksParent.GetComponentsInChildren<UILineRenderer>();
		int num = 0;
		UILineRenderer[] array = componentsInChildren;
		foreach (UILineRenderer uILineRenderer in array)
		{
			if (uILineRenderer != null)
			{
				try
				{
					uILineRenderer.ForceSync();
					num++;
				}
				catch (Exception ex)
				{
					DebugLogError("[TreeCreator] Error during line recovery for " + uILineRenderer.gameObject.name + ": " + ex.Message);
				}
			}
		}
	}

	public void InitLinksDicts()
	{
		createdLinks_gameObjects = new Dictionary<(GameObject, GameObject), UILineRenderer>();
		createdLinks_infos = new Dictionary<(TreeNodeInfo, TreeNodeInfo), UILineRenderer>();
		if (linksParent == null)
		{
			return;
		}
		UILineRenderer[] componentsInChildren = linksParent.GetComponentsInChildren<UILineRenderer>();
		foreach (UILineRenderer uILineRenderer in componentsInChildren)
		{
			if (uILineRenderer != null)
			{
				TreeLink component = uILineRenderer.GetComponent<TreeLink>();
				if (component != null && component.FromNode != null && component.ToNode != null)
				{
					createdLinks_infos.Add((component.FromNode.GetComponent<TreeNode>().NodeInfo, component.ToNode.GetComponent<TreeNode>().NodeInfo), uILineRenderer);
					createdLinks_gameObjects.Add((component.FromNode, component.ToNode), uILineRenderer);
				}
			}
		}
	}

	private void CreateLinks()
	{
		if (treeAsset.TreeNodes == null)
		{
			return;
		}
		createdLinks_gameObjects = new Dictionary<(GameObject, GameObject), UILineRenderer>();
		createdLinks_infos = new Dictionary<(TreeNodeInfo, TreeNodeInfo), UILineRenderer>();
		foreach (TreeNodeInfo treeNode in treeAsset.TreeNodes)
		{
			if (treeNode.NodeConnections == null)
			{
				continue;
			}
			foreach (NodeConnection nodeConnection in treeNode.NodeConnections)
			{
				if (nodeConnection.targetNode != null)
				{
					(TreeNodeInfo, TreeNodeInfo) key = ((treeNode.GetInstanceID() < nodeConnection.targetNode.GetInstanceID()) ? (treeNode, nodeConnection.targetNode) : (nodeConnection.targetNode, treeNode));
					if (!createdLinks_infos.ContainsKey(key))
					{
						UILineRenderer value = CreateLink(treeNode, nodeConnection.targetNode, nodeConnection.curveAmount);
						createdLinks_infos.Add(key, value);
						createdLinks_gameObjects.Add((instantiatedNodes[treeNode], instantiatedNodes[nodeConnection.targetNode]), value);
					}
				}
			}
		}
	}

	private UILineRenderer CreateLink(TreeNodeInfo fromNode, TreeNodeInfo toNode, float curveAmount)
	{
		if (!instantiatedNodes.ContainsKey(fromNode) || !instantiatedNodes.ContainsKey(toNode))
		{
			return null;
		}
		if (linksParent == null)
		{
			DebugLogWarning("[TreeCreator] linksParent is null, creating parent objects");
			CreateParentObjects();
		}
		if (linksParent == null)
		{
			DebugLogError("[TreeCreator] Failed to create linksParent, cannot create link");
			return null;
		}
		GameObject fromNode2 = instantiatedNodes[fromNode];
		GameObject toNode2 = instantiatedNodes[toNode];
		GameObject gameObject = UnityEngine.Object.Instantiate(linkPrefab, linksParent);
		gameObject.name = "Link_" + fromNode.NodeName + "_to_" + toNode.NodeName;
		SetupLink(gameObject, fromNode2, toNode2, curveAmount);
		return gameObject.GetComponent<UILineRenderer>();
	}

	private bool ShouldLinkBeLocked(GameObject fromNode, GameObject toNode)
	{
		TreeNodeInfo nodeInfoFromGameObject = GetNodeInfoFromGameObject(fromNode);
		TreeNodeInfo nodeInfoFromGameObject2 = GetNodeInfoFromGameObject(toNode);
		if (nodeInfoFromGameObject == null || nodeInfoFromGameObject2 == null)
		{
			DebugLogWarning("[TreeCreator] Could not find TreeNodeInfo for connected nodes, defaulting to locked");
			return true;
		}
		bool num = treeState.IsNodeUnlocked(nodeInfoFromGameObject);
		bool flag = treeState.IsNodeUnlocked(nodeInfoFromGameObject2);
		return !(num && flag);
	}

	public void UpdateAllLinkColors()
	{
		if (linksParent == null)
		{
			return;
		}
		UILineRenderer[] componentsInChildren = linksParent.GetComponentsInChildren<UILineRenderer>();
		foreach (UILineRenderer uILineRenderer in componentsInChildren)
		{
			if (uILineRenderer != null)
			{
				TreeLink component = uILineRenderer.GetComponent<TreeLink>();
				if (component != null && component.FromNode != null && component.ToNode != null)
				{
					bool isLocked = ShouldLinkBeLocked(component.FromNode, component.ToNode);
					uILineRenderer.IsLocked = isLocked;
				}
			}
		}
	}

	public void UpdateLinkColor(TreeNodeInfo FromNode, TreeNodeInfo ToNode)
	{
		if (createdLinks_infos.ContainsKey((FromNode, ToNode)))
		{
			bool isLocked = ShouldLinkBeLocked(instantiatedNodes[FromNode], instantiatedNodes[ToNode]);
			createdLinks_infos[(FromNode, ToNode)].IsLocked = isLocked;
		}
		if (createdLinks_infos.ContainsKey((ToNode, FromNode)))
		{
			bool isLocked2 = ShouldLinkBeLocked(instantiatedNodes[ToNode], instantiatedNodes[FromNode]);
			createdLinks_infos[(ToNode, FromNode)].IsLocked = isLocked2;
		}
	}

	public void ShowHideLink(TreeNodeInfo FromNode, TreeNodeInfo ToNode, bool show)
	{
		if (createdLinks_infos.ContainsKey((FromNode, ToNode)))
		{
			createdLinks_infos[(FromNode, ToNode)].gameObject.SetActive(show);
		}
		if (createdLinks_infos.ContainsKey((ToNode, FromNode)))
		{
			createdLinks_infos[(ToNode, FromNode)].gameObject.SetActive(show);
		}
	}

	private void SetupLink(GameObject linkInstance, GameObject fromNode, GameObject toNode, float curveAmount)
	{
		if (!(linkInstance.GetComponent<UILineRenderer>() != null) && (!(linkInstance.GetComponent<TreeLink>() != null) || !(fromNode.GetComponent<RectTransform>() != null)))
		{
			Vector3 position = fromNode.transform.position;
			Vector3 position2 = toNode.transform.position;
			Vector3 position3 = (position + position2) * 0.5f;
			linkInstance.transform.position = position3;
		}
		else
		{
			linkInstance.transform.localPosition = Vector3.zero;
		}
		TreeLink component = linkInstance.GetComponent<TreeLink>();
		if (component != null)
		{
			component.SetupLink(fromNode, toNode, curveAmount);
			return;
		}
		UILineRenderer component2 = linkInstance.GetComponent<UILineRenderer>();
		if (component2 != null)
		{
			RectTransform component3 = fromNode.GetComponent<RectTransform>();
			RectTransform component4 = toNode.GetComponent<RectTransform>();
			if (component3 != null && component4 != null)
			{
				bool locked = ShouldLinkBeLocked(fromNode, toNode);
				component2.ConfigureLinkVisuals(defaultLinkSprite, defaultUnlockedLinkColor, defaultLockedLinkColor, locked);
				component2.SetupLine(component3, component4, curveAmount);
			}
			return;
		}
		LineRenderer component5 = linkInstance.GetComponent<LineRenderer>();
		if (component5 != null)
		{
			Vector3 position4 = fromNode.transform.position;
			Vector3 position5 = toNode.transform.position;
			component5.positionCount = 2;
			component5.SetPosition(0, position4);
			component5.SetPosition(1, position5);
			component5.useWorldSpace = true;
		}
	}

	private void RecreateAllLinks()
	{
		CreateParentObjects();
		if (linksParent != null)
		{
			for (int num = linksParent.childCount - 1; num >= 0; num--)
			{
				GameObject gameObject = linksParent.GetChild(num).gameObject;
				UILineRenderer component = gameObject.GetComponent<UILineRenderer>();
				if (component != null)
				{
					try
					{
						component.CleanupLineSegments();
					}
					catch (Exception ex)
					{
						DebugLogError("[TreeCreator] Error cleaning up UILineRenderer during link recreation: " + ex.Message);
					}
				}
				DestroyGameObject(gameObject);
			}
		}
		CreateLinks();
	}

	[ContextMenu("Cleanup Orphaned LineSegments")]
	public void CleanupOrphanedLineSegments()
	{
		CleanupUILineRendererSegments();
		UILineRenderer.CleanupAllTemplatePrefabs();
		CleanupTopLevelOrphanedSegments();
	}

	private void CleanupUILineRendererSegments()
	{
		int num = 0;
		UILineRenderer[] array = UnityEngine.Object.FindObjectsOfType<UILineRenderer>(includeInactive: true);
		foreach (UILineRenderer uILineRenderer in array)
		{
			if (uILineRenderer != null)
			{
				try
				{
					uILineRenderer.CleanupLineSegments();
					num++;
				}
				catch (Exception ex)
				{
					DebugLogError("[TreeCreator] Error cleaning up UILineRenderer: " + ex.Message);
				}
			}
		}
	}

	private void CleanupTopLevelOrphanedSegments()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>(includeInactive: true);
		int num = 0;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.name.Contains("LineSegment"))
			{
				Transform parent = gameObject.transform.parent;
				bool flag = false;
				if (parent == null)
				{
					flag = true;
				}
				else if (parent.name.Contains("Canvas"))
				{
					flag = true;
				}
				else if (parent == gameObject.transform.root)
				{
					flag = true;
				}
				else if (parent.GetComponent<UILineRenderer>() == null)
				{
					flag = true;
				}
				if (flag)
				{
					DebugLogWarning("[TreeCreator] Found orphaned LineSegment: " + gameObject.name + " with parent: " + ((parent != null) ? parent.name : "null") + ", destroying it");
					DestroyGameObject(gameObject);
					num++;
				}
			}
		}
	}

	private void CreateNodes()
	{
		if (treeAsset.TreeNodes == null || treeAsset.TreeNodes.Count == 0)
		{
			DebugLogWarning("TreeCreator: No nodes found in the tree asset!");
			return;
		}
		foreach (TreeNodeInfo treeNode in treeAsset.TreeNodes)
		{
			AddInstantiatedNode(treeNode);
		}
	}

	private void AddInstantiatedNode(TreeNodeInfo treeNode)
	{
		if (instantiatedNodes.ContainsKey(treeNode))
		{
			DebugLogWarning("[TreeCreator] Attempted to add node " + treeNode.NodeName + " that already exists. Skipping creation.");
			return;
		}
		if (nodesParent == null)
		{
			CreateParentObjects();
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(nodePrefab, nodesParent);
		gameObject.name = treeNode.NodeName;
		Vector2 vector = CalculateNodeWorldPosition(treeNode);
		gameObject.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
		instantiatedNodes[treeNode] = gameObject;
		Transform transform = gameObject.transform.Find("NodeShape");
		Transform transform2 = gameObject.transform.Find("NodeIcon");
		if (transform != null)
		{
			Image component = transform.GetComponent<Image>();
			if (component != null)
			{
				instantiatedNodes_Shapes[treeNode] = component;
			}
		}
		if (transform2 != null)
		{
			Image component2 = transform2.GetComponent<Image>();
			if (component2 != null)
			{
				instantiatedNodes_Icons[treeNode] = component2;
			}
		}
		SetupNodeVisuals(treeNode);
		SaveInstantiatedNodesToSerialization();
	}

	private void UpdateInstantiatedNode(TreeNodeInfo treeNode)
	{
		if (instantiatedNodes.ContainsKey(treeNode))
		{
			GameObject obj = instantiatedNodes[treeNode];
			obj.name = treeNode.NodeName;
			Vector2 vector = CalculateNodeWorldPosition(treeNode);
			obj.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
			SetupNodeVisuals(treeNode);
		}
	}

	private void RemoveInstantiatedNode(TreeNodeInfo treeNode)
	{
		if (instantiatedNodes.ContainsKey(treeNode))
		{
			GameObject gameObject = instantiatedNodes[treeNode];
			instantiatedNodes.Remove(treeNode);
			if (instantiatedNodes_Shapes.ContainsKey(treeNode))
			{
				instantiatedNodes_Shapes.Remove(treeNode);
			}
			if (instantiatedNodes_Icons.ContainsKey(treeNode))
			{
				instantiatedNodes_Icons.Remove(treeNode);
			}
			SaveInstantiatedNodesToSerialization();
			if (gameObject != null)
			{
				DestroyGameObject(gameObject);
			}
		}
	}

	private void SetupNodeVisuals(TreeNodeInfo treeNode)
	{
		SetupTreeNodeComponent(treeNode);
		UpdateNodeVisuals(treeNode);
		UpdateNodeShapeSize(treeNode);
	}

	private void SetupTreeNodeComponent(TreeNodeInfo treeNode)
	{
		TreeNode treeNode2 = instantiatedNodes[treeNode].GetComponent<TreeNode>();
		if (treeNode2 == null)
		{
			if (!(Type.GetType("TreeNode") != null))
			{
				DebugLog("[TreeCreator] TreeNode component type not found - make sure TreeNode.cs is compiled");
				return;
			}
			treeNode2 = instantiatedNodes[treeNode].AddComponent<TreeNode>();
		}
		if (treeNode2 != null)
		{
			treeNode2.Initialize(treeNode);
		}
	}

	public void UpdateNodeVisuals(TreeNodeInfo treeNode)
	{
		Image image = GetNodeShapeImage(treeNode);
		Image image2 = GetNodeIconImage(treeNode);
		if (image == null)
		{
			image = instantiatedNodes[treeNode].transform.Find("NodeShape")?.GetComponent<Image>();
		}
		if (image2 == null)
		{
			image2 = instantiatedNodes[treeNode].transform.Find("NodeIcon")?.GetComponent<Image>();
		}
		bool isUnlocked = true;
		if (Application.isPlaying && treeState != null)
		{
			isUnlocked = treeState.IsNodeUnlocked(treeNode);
		}
		if (treeNode.NodeShape != null)
		{
			image.sprite = treeNode.NodeShape;
		}
		image.color = ((treeState != null) ? treeState.GetCurrentShapeColor(treeNode, treeState.GetNodeLevel(treeNode)) : treeNode.AvailableShapeColor);
		if (treeState != null && treeState.GetNodeLevel(treeNode) >= treeNode.NodeMaxLevel)
		{
			image.sprite = (treeNode.NodeCostCurrencies.Contains(Currencies.ClearCurrency) ? FullyGainedNodeShape_Boss : FullyGainedNodeShape_Normal);
		}
		else
		{
			image.sprite = (treeNode.NodeCostCurrencies.Contains(Currencies.ClearCurrency) ? DefaultNodeShape_Boss : DefaultNodeShape_Normal);
		}
		if (treeNode.NodeIcon != null)
		{
			image2.sprite = treeNode.NodeIcon;
			FunctionsNeeded.ConstrainImageSize(image2.gameObject.GetComponent<RectTransform>(), image2, 40f, 40f);
			image2.color = treeNode.GetCurrentIconColor(isUnlocked);
			image2.gameObject.SetActive(value: true);
		}
		else
		{
			image2.gameObject.SetActive(value: false);
		}
		if (instantiatedNodes.ContainsKey(treeNode))
		{
			instantiatedNodes[treeNode].GetComponent<TreeNode>().ManageMySize();
		}
	}

	public void UpdateAllNodeShapeColors()
	{
		foreach (TreeNodeInfo key in instantiatedNodes.Keys)
		{
			if (treeState != null && treeState.GetNodeLevel(key) < key.NodeMaxLevel)
			{
				UpdateNodeShapeColor(key);
			}
		}
	}

	public void UpdateAllNodeShapeSizes()
	{
		foreach (TreeNodeInfo key in instantiatedNodes.Keys)
		{
			UpdateNodeShapeSize(key);
		}
	}

	public void UpdateAllNodeColors()
	{
		if (nodesParent == null || !Application.isPlaying)
		{
			return;
		}
		int num = 0;
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (instantiatedNode.Key != null && instantiatedNode.Value != null)
			{
				UpdateNodeVisuals(instantiatedNode.Key);
				num++;
			}
		}
	}

	public Image GetNodeShapeImage(TreeNodeInfo treeNode)
	{
		if (instantiatedNodes_Shapes.ContainsKey(treeNode))
		{
			return instantiatedNodes_Shapes[treeNode];
		}
		return null;
	}

	public Image GetNodeIconImage(TreeNodeInfo treeNode)
	{
		if (instantiatedNodes_Icons.ContainsKey(treeNode))
		{
			return instantiatedNodes_Icons[treeNode];
		}
		return null;
	}

	public void UpdateNodeShapeColor(TreeNodeInfo treeNode)
	{
		Image nodeShapeImage = GetNodeShapeImage(treeNode);
		if (nodeShapeImage != null && treeState != null)
		{
			nodeShapeImage.color = treeState.GetCurrentShapeColor(treeNode, treeState.GetNodeLevel(treeNode));
			if (treeState.GetNodeLevel(treeNode) >= treeNode.NodeMaxLevel)
			{
				nodeShapeImage.sprite = (treeNode.NodeCostCurrencies.Contains(Currencies.ClearCurrency) ? FullyGainedNodeShape_Boss : FullyGainedNodeShape_Normal);
			}
			else
			{
				nodeShapeImage.sprite = (treeNode.NodeCostCurrencies.Contains(Currencies.ClearCurrency) ? DefaultNodeShape_Boss : DefaultNodeShape_Normal);
			}
		}
	}

	public void UpdateNodeIconColor(TreeNodeInfo treeNode)
	{
		Image nodeIconImage = GetNodeIconImage(treeNode);
		if (nodeIconImage != null && treeState != null)
		{
			bool isUnlocked = treeState.IsNodeUnlocked(treeNode);
			nodeIconImage.color = treeNode.GetCurrentIconColor(isUnlocked);
		}
	}

	public void UpdateNodeShapeSprite(TreeNodeInfo treeNode)
	{
		Image nodeShapeImage = GetNodeShapeImage(treeNode);
		if (nodeShapeImage != null && treeNode.NodeShape != null)
		{
			nodeShapeImage.sprite = treeNode.NodeShape;
		}
	}

	public void UpdateNodeShapeSize(TreeNodeInfo treeNode)
	{
		if (!instantiatedNodes.ContainsKey(treeNode))
		{
			return;
		}
		Transform transform = instantiatedNodes[treeNode].transform.Find("NodeShape");
		if (transform != null)
		{
			RectTransform component = transform.GetComponent<RectTransform>();
			if (component != null)
			{
				Vector2 sizeDelta = new Vector2(treeNode.ShapeSize, treeNode.ShapeSize);
				component.sizeDelta = sizeDelta;
			}
		}
	}

	public void UpdateNodeIconSprite(TreeNodeInfo treeNode)
	{
		Image nodeIconImage = GetNodeIconImage(treeNode);
		if (nodeIconImage != null)
		{
			if (treeNode.NodeIcon != null)
			{
				nodeIconImage.sprite = treeNode.NodeIcon;
				FunctionsNeeded.ConstrainImageSize(nodeIconImage.gameObject.GetComponent<RectTransform>(), nodeIconImage, 35f, 35f);
				nodeIconImage.gameObject.SetActive(value: true);
			}
			else
			{
				nodeIconImage.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetupAllTreeNodeComponents()
	{
		if (nodesParent == null)
		{
			DebugLogWarning("[TreeCreator] No nodes parent found");
			return;
		}
		int num = 0;
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (instantiatedNode.Key != null && instantiatedNode.Value != null)
			{
				SetupTreeNodeComponent(instantiatedNode.Key);
				num++;
			}
		}
	}

	public void RefreshAllTreeNodeComponents()
	{
		if (nodesParent == null)
		{
			DebugLogWarning("[TreeCreator] No nodes parent found");
			return;
		}
		int num = 0;
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (!(instantiatedNode.Key != null) || !(instantiatedNode.Value != null))
			{
				continue;
			}
			UnityEngine.Component component = instantiatedNode.Value.GetComponent("TreeNode");
			if (component != null)
			{
				MethodInfo method = component.GetType().GetMethod("RefreshVisuals");
				if (method != null)
				{
					method.Invoke(component, null);
					num++;
				}
			}
		}
	}

	private void ClearExistingTree()
	{
		instantiatedNodes.Clear();
		instantiatedNodes_Shapes.Clear();
		instantiatedNodes_Icons.Clear();
		if (linksParent != null)
		{
			for (int num = linksParent.childCount - 1; num >= 0; num--)
			{
				GameObject gameObject = linksParent.GetChild(num).gameObject;
				UILineRenderer component = gameObject.GetComponent<UILineRenderer>();
				if (component != null)
				{
					try
					{
						component.CleanupLineSegments();
					}
					catch (Exception ex)
					{
						DebugLogError("[TreeCreator] Error cleaning up UILineRenderer during tree clear: " + ex.Message);
					}
				}
				DestroyGameObject(gameObject);
			}
		}
		if (nodesParent != null)
		{
			for (int num2 = nodesParent.childCount - 1; num2 >= 0; num2--)
			{
				DestroyGameObject(nodesParent.GetChild(num2).gameObject);
			}
		}
		CleanupOrphanedLineSegments();
	}

	private void SubscribeToTreeEvents()
	{
		UnsubscribeFromTreeEvents();
		TreeInfo.OnTreeChanged += OnTreeChanged;
		TreeInfo.OnNodeAdded += OnNodeAdded;
		TreeInfo.OnNodeRemoved += OnNodeRemoved;
		TreeInfo.OnTreeStructureChanged += OnTreeStructureChanged;
		TreeNodeInfo.OnNodePropertyChanged += OnNodePropertyChanged;
		TreeNodeInfo.OnNodePositionChanged += OnNodePositionChanged;
		TreeNodeInfo.OnNodeConnectionsChanged += OnNodeConnectionsChanged;
		lastTrackedTree = treeAsset;
	}

	private void UnsubscribeFromTreeEvents()
	{
		TreeInfo.OnTreeChanged -= OnTreeChanged;
		TreeInfo.OnNodeAdded -= OnNodeAdded;
		TreeInfo.OnNodeRemoved -= OnNodeRemoved;
		TreeInfo.OnTreeStructureChanged -= OnTreeStructureChanged;
		TreeNodeInfo.OnNodePropertyChanged -= OnNodePropertyChanged;
		TreeNodeInfo.OnNodePositionChanged -= OnNodePositionChanged;
		TreeNodeInfo.OnNodeConnectionsChanged -= OnNodeConnectionsChanged;
	}

	private void HandleRealtimeSync()
	{
		if (!(treeAsset == null) && enableRealtimeSync)
		{
			if (lastTrackedTree != treeAsset)
			{
				SubscribeToTreeEvents();
			}
			pendingSyncUpdate = false;
			SyncWithTreeData();
		}
	}

	private void SyncWithTreeData()
	{
		if (treeAsset == null || !ValidateConfiguration())
		{
			return;
		}
		isSyncing = true;
		CreateParentObjects();
		HashSet<TreeNodeInfo> hashSet = new HashSet<TreeNodeInfo>();
		if (treeAsset.TreeNodes != null)
		{
			foreach (TreeNodeInfo treeNode in treeAsset.TreeNodes)
			{
				if (treeNode != null)
				{
					hashSet.Add(treeNode);
				}
			}
		}
		List<TreeNodeInfo> list = new List<TreeNodeInfo>();
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (!hashSet.Contains(instantiatedNode.Key))
			{
				list.Add(instantiatedNode.Key);
			}
		}
		foreach (TreeNodeInfo item in list)
		{
			RemoveInstantiatedNode(item);
		}
		if (treeAsset.TreeNodes != null)
		{
			int num = 0;
			int num2 = 0;
			foreach (TreeNodeInfo treeNode2 in treeAsset.TreeNodes)
			{
				if (treeNode2 == null)
				{
					DebugLogWarning("[TreeCreator] Skipping null node in tree data");
				}
				else if (instantiatedNodes.ContainsKey(treeNode2))
				{
					if (instantiatedNodes[treeNode2] == null)
					{
						DebugLogWarning("[TreeCreator] Node " + treeNode2.NodeName + " was in dictionary but GameObject was destroyed. Re-creating with minimal impact.");
						instantiatedNodes.Remove(treeNode2);
						AddInstantiatedNode(treeNode2);
						num++;
					}
					else
					{
						UpdateInstantiatedNode(treeNode2);
						num2++;
					}
				}
				else
				{
					AddInstantiatedNode(treeNode2);
					num++;
				}
			}
		}
		RecreateAllLinks();
		if (centerTree)
		{
			ApplyCentering();
		}
		else
		{
			ApplyUncentering();
		}
		isSyncing = false;
	}

	private void ScheduleSyncUpdate()
	{
		if (enableRealtimeSync && treeAsset != null)
		{
			float num = Time.time - lastSyncTime;
			if (!pendingSyncUpdate || !(num < 0.1f))
			{
				pendingSyncUpdate = true;
				lastSyncTime = Time.time;
			}
		}
	}

	private bool ShouldPerformFullSync()
	{
		if (treeAsset == null || treeAsset.TreeNodes == null)
		{
			return false;
		}
		if (instantiatedNodes.Count == 0 && treeAsset.TreeNodes.Count > 0)
		{
			return true;
		}
		int num = 0;
		foreach (TreeNodeInfo treeNode in treeAsset.TreeNodes)
		{
			if (treeNode != null)
			{
				num++;
			}
		}
		if (instantiatedNodes.Count != num)
		{
			return true;
		}
		int num2 = 0;
		foreach (KeyValuePair<TreeNodeInfo, GameObject> instantiatedNode in instantiatedNodes)
		{
			if (instantiatedNode.Key == null || instantiatedNode.Value == null)
			{
				num2++;
			}
		}
		if (num2 > 0)
		{
			return true;
		}
		return false;
	}
}
[Serializable]
public enum InaccessibleNodeDisplayMode
{
	Hide,
	Show
}
[CreateAssetMenu]
public class TreeInfo : ScriptableObject
{
	public string TreeName;

	public List<TreeNodeInfo> TreeNodes;

	public Vector2 FirstNodePosition;

	[Header("Node Creation Counter")]
	[SerializeField]
	private int totalNodesCreated;

	private bool _eventsEnabled = true;

	public static event Action<TreeInfo> OnTreeChanged;

	public static event Action<TreeInfo, TreeNodeInfo> OnNodeAdded;

	public static event Action<TreeInfo, TreeNodeInfo> OnNodeRemoved;

	public static event Action<TreeInfo> OnTreeStructureChanged;

	public void DisableEvents()
	{
		_eventsEnabled = false;
	}

	public void EnableEventsAndNotify()
	{
		_eventsEnabled = true;
		NotifyTreeChanged();
	}

	public void NotifyTreeChanged()
	{
		if (_eventsEnabled)
		{
			TreeInfo.OnTreeChanged?.Invoke(this);
		}
	}

	public void NotifyNodeAdded(TreeNodeInfo node)
	{
		if (_eventsEnabled)
		{
			TreeInfo.OnNodeAdded?.Invoke(this, node);
		}
	}

	public void NotifyNodeRemoved(TreeNodeInfo node)
	{
		if (_eventsEnabled)
		{
			TreeInfo.OnNodeRemoved?.Invoke(this, node);
		}
	}

	public void NotifyTreeStructureChanged()
	{
		if (_eventsEnabled)
		{
			TreeInfo.OnTreeStructureChanged?.Invoke(this);
		}
	}

	public int GetNextNodeNumber()
	{
		totalNodesCreated++;
		return totalNodesCreated;
	}

	public int GetTotalNodesCreated()
	{
		return totalNodesCreated;
	}
}
public class TreeLink : MonoBehaviour
{
	[SerializeField]
	private GameObject fromNode;

	[SerializeField]
	private GameObject toNode;

	[SerializeField]
	private float curveAmount;

	private UILineRenderer uiLineRenderer;

	private LineRenderer lineRenderer;

	public GameObject FromNode => fromNode;

	public GameObject ToNode => toNode;

	public float CurveAmount => curveAmount;

	public void SetupLink(GameObject fromNode, GameObject toNode, float curveAmount)
	{
		this.fromNode = fromNode;
		this.toNode = toNode;
		this.curveAmount = curveAmount;
		if (uiLineRenderer == null)
		{
			uiLineRenderer = GetComponent<UILineRenderer>();
		}
		if (lineRenderer == null)
		{
			lineRenderer = GetComponent<LineRenderer>();
		}
		CreateLinkVisuals();
	}

	private void CreateLinkVisuals()
	{
		if (fromNode == null || toNode == null)
		{
			return;
		}
		if (uiLineRenderer != null)
		{
			RectTransform component = fromNode.GetComponent<RectTransform>();
			RectTransform component2 = toNode.GetComponent<RectTransform>();
			if (component != null && component2 != null)
			{
				uiLineRenderer.SetupLine(component, component2, curveAmount);
				return;
			}
		}
		if (lineRenderer != null)
		{
			CreateLineRenderer();
		}
	}

	private void CreateLineRenderer()
	{
		Vector3 position = fromNode.transform.position;
		Vector3 position2 = toNode.transform.position;
		if (Mathf.Abs(curveAmount) < 0.001f)
		{
			lineRenderer.positionCount = 2;
			lineRenderer.SetPosition(0, position);
			lineRenderer.SetPosition(1, position2);
			return;
		}
		int num = 20;
		lineRenderer.positionCount = num + 1;
		Vector3 normalized = (position2 - position).normalized;
		Vector3 vector = new Vector3(0f - normalized.y, normalized.x, 0f);
		float num2 = Vector3.Distance(position, position2);
		Vector3 vector2 = (position + position2) * 0.5f;
		float num3 = num2 / 2f;
		float num4 = Mathf.Abs(curveAmount) / 10f * num3;
		bool flag = curveAmount < 0f;
		Vector3 vector3 = (flag ? (-vector) : vector);
		Vector3 p = vector2 + vector3 * num4;
		Vector2 p2 = new Vector2(position.x, position.y);
		Vector2 p3 = new Vector2(position2.x, position2.y);
		Vector2 p4 = new Vector2(p.x, p.y);
		if (CalculateCircleFromThreePoints(p2, p4, p3, out var center, out var radius))
		{
			Vector3 vector4 = new Vector3(center.x, center.y, position.z);
			float num5 = Mathf.Atan2(position.y - vector4.y, position.x - vector4.x);
			float num6 = Mathf.Atan2(position2.y - vector4.y, position2.x - vector4.x);
			float num7 = Mathf.Atan2(p.y - vector4.y, p.x - vector4.x);
			float num8 = NormalizeAngle(num7 - num5);
			float num9 = NormalizeAngle(num6 - num7);
			bool flag2 = false;
			if ((!flag) ? ((num8 < 0f && num9 < 0f) || num8 > MathF.PI || num9 > MathF.PI) : ((num8 > 0f && num9 > 0f) || num8 < -MathF.PI || num9 < -MathF.PI))
			{
				float num10 = num5;
				num5 = num6;
				num6 = num10;
			}
			for (int i = 0; i <= num; i++)
			{
				float num11 = (float)i / (float)num;
				float f;
				if (flag)
				{
					float num12 = num6 - num5;
					if (num12 > 0f)
					{
						num12 -= MathF.PI * 2f;
					}
					f = num5 + num11 * num12;
				}
				else
				{
					float num13 = num6 - num5;
					if (num13 < 0f)
					{
						num13 += MathF.PI * 2f;
					}
					f = num5 + num11 * num13;
				}
				Vector3 position3 = vector4 + new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f) * radius;
				lineRenderer.SetPosition(i, position3);
			}
		}
		else
		{
			for (int j = 0; j <= num; j++)
			{
				float t = (float)j / (float)num;
				Vector3 position4 = QuadraticBezier(position, p, position2, t);
				lineRenderer.SetPosition(j, position4);
			}
		}
	}

	private bool CalculateCircleFromThreePoints(Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 center, out float radius)
	{
		center = Vector2.zero;
		radius = 0f;
		Vector2 vector = (p1 + p2) * 0.5f;
		Vector2 vector2 = (p2 + p3) * 0.5f;
		Vector2 normalized = (p2 - p1).normalized;
		Vector2 normalized2 = (p3 - p2).normalized;
		Vector2 vector3 = new Vector2(0f - normalized.y, normalized.x);
		Vector2 vector4 = new Vector2(0f - normalized2.y, normalized2.x);
		float num = vector3.x * vector4.y - vector3.y * vector4.x;
		if (Mathf.Abs(num) < 0.001f)
		{
			return false;
		}
		Vector2 vector5 = vector2 - vector;
		float num2 = (vector5.x * vector4.y - vector5.y * vector4.x) / num;
		center = vector + num2 * vector3;
		radius = Vector2.Distance(center, p1);
		return true;
	}

	private float NormalizeAngle(float angle)
	{
		while (angle > MathF.PI)
		{
			angle -= MathF.PI * 2f;
		}
		while (angle < -MathF.PI)
		{
			angle += MathF.PI * 2f;
		}
		return angle;
	}

	private Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		float num = 1f - t;
		return num * num * p0 + 2f * num * t * p1 + t * t * p2;
	}
}
public class TreeManager : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}
}
public class TreeNode : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	[Header("Node Data")]
	[SerializeField]
	private TreeNodeInfo nodeInfo;

	[Header("Debug")]
	[SerializeField]
	private bool enableDebugLogging;

	private TreeCreator treeCreator;

	private TreeState treeState;

	private bool isHovering;

	private bool isClicking;

	public Action<TreeNode> OnNodeHoverEnter;

	public Action<TreeNode> OnNodeHoverExit;

	public Action<TreeNode> OnNodeClicked;

	public Action<TreeNode> OnNodeDoubleClicked;

	public TreeNodeInfo NodeInfo => nodeInfo;

	public TreeCreator TreeCreator => treeCreator;

	public TreeState TreeState => treeState;

	public bool IsHovering => isHovering;

	private void Start()
	{
		treeCreator = GetComponentInParent<TreeCreator>();
		if (treeCreator != null)
		{
			treeState = treeCreator.GetComponent<TreeState>();
		}
		if (treeCreator == null)
		{
			DebugLogWarning("[TreeNode] TreeCreator not found in parent!");
		}
		if (treeState == null)
		{
			DebugLogWarning("[TreeNode] TreeState not found!");
		}
	}

	public void Initialize(TreeNodeInfo nodeInfo)
	{
		Start();
		this.nodeInfo = nodeInfo;
		treeCreator.UpdateNodeVisuals(nodeInfo);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovering = true;
		UpdateTooltip();
		OnNodeHoverEnter?.Invoke(this);
		FXManager.instance.PlayGeneralSound(GeneralSounds.generic_ui_hover);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovering = false;
		isClicking = false;
		treeCreator.tooltip.HideTooltip();
		OnNodeHoverExit?.Invoke(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (playerData.instance.UnlockedSystems[nodeInfo.RequiredUnlockableSystem] && playerData.instance.MonstersLevel >= nodeInfo.RequiredMonsterLevel)
		{
			if (eventData.clickCount == 2)
			{
				OnNodeDoubleClicked?.Invoke(this);
			}
			else if (treeState.GetNodeLevel(nodeInfo) < nodeInfo.NodeMaxLevel && treeState.IsHaveEnoughCurrenciesToPurchase(nodeInfo, isSpendAlso: true).isCanSpend)
			{
				FXManager.instance.PlayUIClickSound();
				treeState.IncreaseNodeLevel(nodeInfo);
				UpdateTooltip();
				OnNodeClicked?.Invoke(this);
				ManageMySize();
			}
		}
	}

	public void ManageMySize()
	{
		Transform transform = base.transform.Find("NodeShape");
		Transform transform2 = base.transform.Find("NodeIcon");
		if (transform != null && transform2 != null)
		{
			if (playerData.instance.TreeNodeLevels[nodeInfo.NodeID] >= nodeInfo.NodeMaxLevel)
			{
				transform.localScale = 0.8f * Vector3.one;
				transform2.localScale = 0.8f * Vector3.one;
			}
			else
			{
				transform.localScale = Vector3.one;
				transform2.localScale = Vector3.one;
			}
		}
	}

	private void UpdateTooltip()
	{
		if (isHovering)
		{
			treeCreator.tooltip.ShowTooltip(nodeInfo, base.transform.position, 85f);
		}
	}

	public string GetNodeName()
	{
		if (nodeInfo != null)
		{
			return nodeInfo.NodeName;
		}
		return base.gameObject.name;
	}

	private void DebugLog(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	private void DebugLogWarning(string message)
	{
		UnityEngine.Debug.LogWarning(message);
	}
}
[Serializable]
public enum NodeUnlockBehavior
{
	StartUnlocked,
	AlwaysUnlockable,
	RequiresConnection
}
[Serializable]
public class NodeConnection
{
	public TreeNodeInfo targetNode;

	public float curveAmount;

	public NodeConnection(TreeNodeInfo target, float curve = 0f)
	{
		targetNode = target;
		curveAmount = curve;
	}
}
[CreateAssetMenu]
public class TreeNodeInfo : ScriptableObject
{
	[Header("General Info")]
	[SerializeField]
	private string nodeID;

	public string NodeName;

	public string NodeDescription;

	[Header("Example: 1 + (x-1)^2  x is the level of the node")]
	public List<string> NodeValueEquation;

	public List<string> NodeCostEquation;

	public List<Currencies> NodeCostCurrencies;

	public Sprite NodeShape;

	public Sprite NodeIcon;

	public float ShapeSize = 60f;

	public int NodeMaxLevel = 1;

	public NodeUnlockBehavior UnlockBehavior = NodeUnlockBehavior.RequiresConnection;

	public int RequiredMonsterLevel = -1;

	public UnlockableSystems RequiredUnlockableSystem;

	[SerializeField]
	private List<StatInfo> NodeStat = new List<StatInfo>();

	[Header("Node Shape Availability Colors")]
	public Color AvailableShapeColor = Color.white;

	public Color UnavailableShapeColor = Color.gray;

	public Color MaxLevelShapeColor = Color.red;

	[Header("Node Icon Lock Colors")]
	public Color UnlockedIconColor = Color.white;

	public Color LockedIconColor = Color.gray;

	[Header("Connections")]
	public List<NodeConnection> NodeConnections;

	public Vector2 Position;

	private bool _eventsEnabled = true;

	public string NodeID
	{
		get
		{
			if (string.IsNullOrEmpty(nodeID))
			{
				nodeID = GenerateUniqueID();
			}
			return nodeID;
		}
		private set
		{
			nodeID = value;
		}
	}

	public static event Action<TreeNodeInfo> OnNodePropertyChanged;

	public static event Action<TreeNodeInfo> OnNodePositionChanged;

	public static event Action<TreeNodeInfo> OnNodeConnectionsChanged;

	public List<StatInfo> GetNodeStat()
	{
		if (NodeStat == null)
		{
			NodeStat = new List<StatInfo>();
		}
		return NodeStat;
	}

	public void SetNodeStat(List<StatInfo> value)
	{
		if (value != null)
		{
			NodeStat = value;
		}
		else
		{
			UnityEngine.Debug.LogWarning("TreeNodeInfo " + NodeName + ": Attempted to set NodeStat to null - ignoring to prevent data loss");
		}
	}

	public void DisableEvents()
	{
		_eventsEnabled = false;
	}

	public void EnableEventsAndNotify()
	{
		_eventsEnabled = true;
		NotifyPropertyChanged();
	}

	public void NotifyPropertyChanged()
	{
		if (_eventsEnabled)
		{
			TreeNodeInfo.OnNodePropertyChanged?.Invoke(this);
		}
	}

	public void NotifyPositionChanged()
	{
		if (_eventsEnabled)
		{
			TreeNodeInfo.OnNodePositionChanged?.Invoke(this);
		}
	}

	public void NotifyConnectionsChanged()
	{
		if (_eventsEnabled)
		{
			TreeNodeInfo.OnNodeConnectionsChanged?.Invoke(this);
		}
	}

	public List<TreeNodeInfo> GetAllConnectedNodes()
	{
		List<TreeNodeInfo> list = new List<TreeNodeInfo>();
		if (NodeConnections != null)
		{
			foreach (NodeConnection nodeConnection in NodeConnections)
			{
				if (nodeConnection.targetNode != null && !list.Contains(nodeConnection.targetNode))
				{
					list.Add(nodeConnection.targetNode);
				}
			}
		}
		return list;
	}

	public Sprite GetCurrentNodeShape()
	{
		return NodeShape;
	}

	public Sprite GetCurrentNodeIcon()
	{
		return NodeIcon;
	}

	public Color GetCurrentIconColor(bool isUnlocked = true)
	{
		if (!isUnlocked)
		{
			return LockedIconColor;
		}
		return UnlockedIconColor;
	}

	private string GenerateUniqueID()
	{
		if (!string.IsNullOrEmpty(NodeName))
		{
			return NodeName + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
		}
		return "Node_" + Guid.NewGuid().ToString("N").Substring(0, 8);
	}

	public void SetNodeID(string newID)
	{
		if (!string.IsNullOrEmpty(newID))
		{
			nodeID = newID;
		}
	}
}
[RequireComponent(typeof(TreeCreator))]
public class TreeState : MonoBehaviour
{
	private TreeCreator treeCreator;

	public Action<TreeNodeInfo, int> OnNodeLevelChanged;

	public Action<TreeNodeInfo> OnNodeUnlocked;

	public TreeCreator TreeCreator => treeCreator;

	private void Awake()
	{
		treeCreator = GetComponent<TreeCreator>();
		if (treeCreator == null)
		{
			UnityEngine.Debug.LogError("[TreeState] TreeCreator component not found!");
		}
	}

	public TreeNodeInfo GetTreeNodeInfo(string nodeID)
	{
		if (string.IsNullOrEmpty(nodeID))
		{
			UnityEngine.Debug.LogError("[TreeState] NodeID is null or empty");
			return null;
		}
		if (treeCreator?.TreeAsset?.TreeNodes != null)
		{
			return treeCreator.TreeAsset.TreeNodes.FirstOrDefault((TreeNodeInfo node) => node != null && node.NodeID == nodeID);
		}
		UnityEngine.Debug.LogError("[TreeState] TreeNodes list is null");
		return null;
	}

	public void StartTreeSystem()
	{
		InitializeNodeLevels();
	}

	public void InitializeNodeLevels()
	{
		if (treeCreator == null || treeCreator.TreeAsset == null)
		{
			DebugLogWarning("[TreeState] Cannot initialize - TreeCreator or TreeAsset is null");
			return;
		}
		List<TreeNodeInfo> treeNodes = treeCreator.TreeAsset.TreeNodes;
		if (treeNodes == null)
		{
			DebugLogWarning("[TreeState] TreeNodes list is null");
			return;
		}
		foreach (TreeNodeInfo item in treeNodes)
		{
			if (item != null && item.UnlockBehavior == NodeUnlockBehavior.StartUnlocked && playerData.instance.TreeNodeLevels[item.NodeID] == 0)
			{
				SetNodeLevel(item, 1);
			}
		}
	}

	public bool IsNodeAccessible(TreeNodeInfo node)
	{
		if (node == null)
		{
			return false;
		}
		if (GetNodeLevel(node) >= 1)
		{
			return true;
		}
		if (node.UnlockBehavior == NodeUnlockBehavior.AlwaysUnlockable)
		{
			return true;
		}
		foreach (TreeNodeInfo allConnectedNode in node.GetAllConnectedNodes())
		{
			if (allConnectedNode != null && allConnectedNode != node && GetNodeLevel(allConnectedNode) >= 1)
			{
				return true;
			}
		}
		return false;
	}

	public List<TreeNodeInfo> GetAccessibleNodes()
	{
		List<TreeNodeInfo> list = new List<TreeNodeInfo>();
		foreach (string key in playerData.instance.TreeNodeLevels.Keys)
		{
			TreeNodeInfo treeNodeInfo = GetTreeNodeInfo(key);
			if (IsNodeAccessible(treeNodeInfo))
			{
				list.Add(treeNodeInfo);
			}
		}
		return list;
	}

	public int GetNodeLevel(TreeNodeInfo node)
	{
		if (node == null)
		{
			DebugLogWarning("[TreeState] Cannot get level for null node");
			return 0;
		}
		return playerData.instance.TreeNodeLevels[node.NodeID];
	}

	public void SetNodeLevel(TreeNodeInfo node, int level)
	{
		if (node == null)
		{
			DebugLogWarning("[TreeState] Cannot set level for null node");
			return;
		}
		int nodeLevel = GetNodeLevel(node);
		playerData.instance.TreeNodeLevels[node.NodeID] = Mathf.Max(0, level);
		ApplyRemoveStat(node, nodeLevel, playerData.instance.TreeNodeLevels[node.NodeID], isAdd: true);
		treeCreator.UpdateNodeVisuals(node);
		if (node.GetNodeStat()[0].VariableName.Contains("Bounty"))
		{
			GroundClickableManager.instance.CheckBountyUnlocks();
		}
		if (nodeLevel != 0 || playerData.instance.TreeNodeLevels[node.NodeID] < 1)
		{
			return;
		}
		treeCreator.ShowConnectedNodesAndLinks_OnNodeLevelChanged(node);
		List<TreeNodeInfo> allConnectedNodes = node.GetAllConnectedNodes();
		foreach (TreeNodeInfo item in allConnectedNodes)
		{
			if (item != null)
			{
				treeCreator.UpdateNodeVisuals(item);
			}
		}
		foreach (TreeNodeInfo item2 in allConnectedNodes)
		{
			if (item2 != null)
			{
				treeCreator.UpdateLinkColor(node, item2);
			}
		}
	}

	public void IncreaseNodeLevel(TreeNodeInfo node, int amount = 1)
	{
		if (node == null)
		{
			DebugLogWarning("[TreeState] Cannot increase level for null node");
			return;
		}
		int nodeLevel = GetNodeLevel(node);
		if (nodeLevel < node.NodeMaxLevel)
		{
			SetNodeLevel(node, nodeLevel + amount);
		}
	}

	public bool IsNodeUnlocked(TreeNodeInfo node)
	{
		if (node == null)
		{
			return false;
		}
		return playerData.instance.TreeNodeLevels[node.NodeID] > 0;
	}

	public List<TreeNodeInfo> GetUnlockedNodes()
	{
		List<TreeNodeInfo> list = new List<TreeNodeInfo>();
		foreach (KeyValuePair<string, int> treeNodeLevel in playerData.instance.TreeNodeLevels)
		{
			if (treeNodeLevel.Value > 0)
			{
				list.Add(GetTreeNodeInfo(treeNodeLevel.Key));
			}
		}
		return list;
	}

	public (bool isCanSpend, List<bool> isCanSpendCurrencies) IsHaveEnoughCurrenciesToPurchase(TreeNodeInfo nodeInfo, bool isSpendAlso)
	{
		int nodeLevel = GetNodeLevel(nodeInfo);
		if (nodeLevel >= nodeInfo.NodeMaxLevel)
		{
			return (isCanSpend: false, isCanSpendCurrencies: new List<bool>());
		}
		List<double> list = new List<double>();
		for (int i = 0; i < nodeInfo.NodeCostEquation.Count; i++)
		{
			double item = ExpressionEvaluator.Evaluate(nodeInfo.NodeCostEquation[i], nodeLevel + 1);
			list.Add(item);
		}
		return PlayerManager.instance.IsCanSpendCurrencies(list, nodeInfo.NodeCostCurrencies, isSpendAlso);
	}

	public Color GetCurrentShapeColor(TreeNodeInfo node, int level)
	{
		if (level >= node.NodeMaxLevel)
		{
			return node.MaxLevelShapeColor;
		}
		if (playerData.instance.UnlockedSystems[node.RequiredUnlockableSystem] && IsHaveEnoughCurrenciesToPurchase(node, isSpendAlso: false).isCanSpend && playerData.instance.MonstersLevel >= node.RequiredMonsterLevel)
		{
			return node.AvailableShapeColor;
		}
		return node.UnavailableShapeColor;
	}

	private void ApplyRemoveStat(TreeNodeInfo node, int previousLevel, int newLevel, bool isAdd)
	{
		for (int i = 0; i < node.NodeValueEquation.Count; i++)
		{
			double num = ExpressionEvaluator.Evaluate(node.NodeValueEquation[i], previousLevel);
			double value = ExpressionEvaluator.Evaluate(node.NodeValueEquation[i], newLevel) - ((previousLevel > 0) ? num : 0.0);
			playerData.instance.stats.ChangeAStat(node.GetNodeStat()[i].VariableName, node.GetNodeStat()[i].StatsProp, value, isAdd);
		}
	}

	private void DebugLog(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	private void DebugLogWarning(string message)
	{
		UnityEngine.Debug.LogWarning(message);
	}
}
public class UILineRenderer : MonoBehaviour
{
	[Header("Line Settings")]
	[SerializeField]
	private float lineWidth = 2f;

	[SerializeField]
	private int segmentCount = 10;

	[SerializeField]
	private Material lineMaterial;

	[Header("Link Sprite")]
	[SerializeField]
	private Sprite linkSprite;

	[Header("Link Colors")]
	[SerializeField]
	private Color unlockedLinkColor = Color.white;

	[SerializeField]
	private Color lockedLinkColor = Color.gray;

	private bool isLocked;

	private RectTransform fromNode;

	private RectTransform toNode;

	private float curveAmount;

	private GameObject lineSegmentPrefab;

	private List<GameObject> lineSegments = new List<GameObject>();

	[SerializeField]
	[HideInInspector]
	private List<GameObject> serializedLineSegments = new List<GameObject>();

	[SerializeField]
	[HideInInspector]
	private RectTransform serializedFromNode;

	[SerializeField]
	[HideInInspector]
	private RectTransform serializedToNode;

	[SerializeField]
	[HideInInspector]
	private float serializedCurveAmount;

	private RectTransform rectTransform;

	private Canvas parentCanvas;

	public float LineWidth => lineWidth;

	public Color LineColor => CurrentLinkColor;

	public float CurveAmount => curveAmount;

	public Sprite LinkSprite
	{
		get
		{
			return linkSprite;
		}
		set
		{
			linkSprite = value;
			UpdateLineVisuals();
		}
	}

	public Color UnlockedLinkColor
	{
		get
		{
			return unlockedLinkColor;
		}
		set
		{
			unlockedLinkColor = value;
			UpdateLineVisuals();
		}
	}

	public Color LockedLinkColor
	{
		get
		{
			return lockedLinkColor;
		}
		set
		{
			lockedLinkColor = value;
			UpdateLineVisuals();
		}
	}

	public bool IsLocked
	{
		get
		{
			return isLocked;
		}
		set
		{
			isLocked = value;
			UpdateLineVisuals();
		}
	}

	public Color CurrentLinkColor
	{
		get
		{
			if (!isLocked)
			{
				return unlockedLinkColor;
			}
			return lockedLinkColor;
		}
	}

	private Canvas EnsureParentCanvas()
	{
		if (parentCanvas == null)
		{
			parentCanvas = GetComponentInParent<Canvas>();
			if (parentCanvas == null)
			{
				UnityEngine.Debug.LogError("UILineRenderer: No Canvas found in parent hierarchy!");
			}
		}
		return parentCanvas;
	}

	private void DebugLogError(string message)
	{
		UnityEngine.Debug.LogError(message);
	}

	private void CreateDefaultLineSegmentPrefab()
	{
		if (this == null || Equals(null))
		{
			DebugLogError("UILineRenderer: Cannot create default prefab - component has been destroyed");
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			UnityEngine.Debug.LogWarning("UILineRenderer: Cannot create default prefab - GameObject is not active");
			return;
		}
		if (base.transform == null)
		{
			DebugLogError("UILineRenderer: Cannot create default prefab - transform is null");
			return;
		}
		if (lineSegmentPrefab != null && lineSegmentPrefab.name.Contains("Template"))
		{
			if (lineSegmentPrefab.transform.parent == base.transform)
			{
				return;
			}
			UnityEngine.Debug.LogWarning("UILineRenderer: Template prefab exists but is not a child, cleaning up old template");
			try
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(lineSegmentPrefab);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(lineSegmentPrefab);
				}
			}
			catch (Exception ex)
			{
				DebugLogError("UILineRenderer: Error cleaning up old template: " + ex.Message);
			}
			lineSegmentPrefab = null;
		}
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Transform child = base.transform.GetChild(num);
			if (child.name.Contains("LineSegment_Template"))
			{
				try
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(child.gameObject);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(child.gameObject);
					}
				}
				catch (Exception ex2)
				{
					DebugLogError("UILineRenderer: Error cleaning up old template child: " + ex2.Message);
				}
			}
		}
		try
		{
			GameObject gameObject = new GameObject("LineSegment_Template");
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			gameObject.SetActive(value: false);
			gameObject.AddComponent<RectTransform>();
			Image image = gameObject.AddComponent<Image>();
			if (linkSprite != null)
			{
				image.sprite = linkSprite;
			}
			image.color = CurrentLinkColor;
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(lineWidth, lineWidth);
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.zero;
			component.pivot = Vector2.one * 0.5f;
			lineSegmentPrefab = gameObject;
		}
		catch (Exception ex3)
		{
			DebugLogError("UILineRenderer: Failed to create default line segment prefab: " + ex3.Message);
			lineSegmentPrefab = null;
		}
	}

	public void SetupLine(RectTransform fromNode, RectTransform toNode, float curveAmount)
	{
		if (this == null || Equals(null))
		{
			DebugLogError("UILineRenderer: Cannot setup line - component has been destroyed");
			return;
		}
		if (fromNode == null || toNode == null)
		{
			DebugLogError("UILineRenderer: Cannot setup line - fromNode or toNode is null");
			return;
		}
		if (fromNode.Equals(null) || toNode.Equals(null))
		{
			DebugLogError("UILineRenderer: Cannot setup line - fromNode or toNode has been destroyed");
			return;
		}
		if (!base.gameObject.activeInHierarchy || !base.enabled)
		{
			UnityEngine.Debug.LogWarning("UILineRenderer: Cannot setup line - component is not active or enabled");
			return;
		}
		this.fromNode = fromNode;
		this.toNode = toNode;
		this.curveAmount = 0f - curveAmount;
		rectTransform = GetComponent<RectTransform>();
		if (EnsureParentCanvas() == null)
		{
			DebugLogError("UILineRenderer: No Canvas found in parent hierarchy!");
			return;
		}
		if (rectTransform == null)
		{
			DebugLogError("UILineRenderer: RectTransform component is missing!");
			return;
		}
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.sizeDelta = Vector2.zero;
		rectTransform.anchoredPosition = Vector2.zero;
		RestoreLineSegmentsFromSerialization();
		if (lineSegmentPrefab == null)
		{
			CreateDefaultLineSegmentPrefab();
		}
		CreateLine();
	}

	private void OnEnable()
	{
		RestoreLineSegmentsFromSerialization();
	}

	private void OnDestroy()
	{
		ClearLineSegments();
		if (!(lineSegmentPrefab != null) || !lineSegmentPrefab.name.Contains("Template"))
		{
			return;
		}
		try
		{
			if (lineSegmentPrefab.transform.parent == base.transform)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(lineSegmentPrefab);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(lineSegmentPrefab);
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning("UILineRenderer: Template prefab is not a child, destroying manually");
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(lineSegmentPrefab);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(lineSegmentPrefab);
				}
			}
			lineSegmentPrefab = null;
		}
		catch (Exception ex)
		{
			DebugLogError("UILineRenderer: Error cleaning up temporary prefab: " + ex.Message);
		}
	}

	private void OnDisable()
	{
		SaveLineSegmentsToSerialization();
		if (base.gameObject == null || !base.gameObject.activeInHierarchy)
		{
			ClearLineSegments();
		}
	}

	private void CreateLine()
	{
		if (this == null || Equals(null))
		{
			DebugLogError("UILineRenderer: Cannot create line - component has been destroyed");
			return;
		}
		if (fromNode == null || toNode == null)
		{
			DebugLogError("UILineRenderer: Cannot create line - fromNode or toNode is null");
			return;
		}
		if (fromNode.Equals(null) || toNode.Equals(null))
		{
			DebugLogError("UILineRenderer: Cannot create line - fromNode or toNode has been destroyed");
			return;
		}
		if (EnsureParentCanvas() == null)
		{
			DebugLogError("UILineRenderer: Cannot create line - parentCanvas is null");
			return;
		}
		Vector2 canvasPosition = GetCanvasPosition(fromNode);
		Vector2 canvasPosition2 = GetCanvasPosition(toNode);
		Vector2[] points = GenerateLinePoints(canvasPosition, canvasPosition2, curveAmount, segmentCount);
		CreateLineSegments(points);
	}

	private Vector2 GetCanvasPosition(RectTransform nodeRectTransform)
	{
		if (EnsureParentCanvas() == null)
		{
			DebugLogError("UILineRenderer: Cannot get canvas position - no parent canvas found");
			return Vector2.zero;
		}
		Camera cam = parentCanvas.worldCamera;
		if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			cam = null;
		}
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, nodeRectTransform.position);
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, cam, out var localPoint))
		{
			return localPoint;
		}
		Vector3 position = nodeRectTransform.position;
		Vector3 vector = rectTransform.InverseTransformPoint(position);
		return new Vector2(vector.x, vector.y);
	}

	private Vector2[] GenerateLinePoints(Vector2 fromPos, Vector2 toPos, float curveAmount, int segments)
	{
		Vector2[] array = new Vector2[segments + 1];
		if (Mathf.Abs(curveAmount) < 0.001f)
		{
			for (int i = 0; i <= segments; i++)
			{
				float t = (float)i / (float)segments;
				array[i] = Vector2.Lerp(fromPos, toPos, t);
			}
		}
		else
		{
			Vector2 normalized = (toPos - fromPos).normalized;
			Vector2 vector = new Vector2(0f - normalized.y, normalized.x);
			float num = Vector2.Distance(fromPos, toPos);
			Vector2 vector2 = (fromPos + toPos) * 0.5f;
			float num2 = num / 2f;
			float num3 = Mathf.Abs(curveAmount) / 10f * num2;
			bool flag = curveAmount < 0f;
			Vector2 vector3 = (flag ? (-vector) : vector);
			Vector2 vector4 = vector2 + vector3 * num3;
			if (CalculateCircleFromThreePoints(fromPos, vector4, toPos, out var center, out var radius))
			{
				float num4 = Mathf.Atan2(fromPos.y - center.y, fromPos.x - center.x);
				float num5 = Mathf.Atan2(toPos.y - center.y, toPos.x - center.x);
				float num6 = Mathf.Atan2(vector4.y - center.y, vector4.x - center.x);
				float num7 = NormalizeAngle(num6 - num4);
				float num8 = NormalizeAngle(num5 - num6);
				bool flag2 = false;
				if ((!flag) ? ((num7 < 0f && num8 < 0f) || num7 > MathF.PI || num8 > MathF.PI) : ((num7 > 0f && num8 > 0f) || num7 < -MathF.PI || num8 < -MathF.PI))
				{
					float num9 = num4;
					num4 = num5;
					num5 = num9;
				}
				for (int j = 0; j <= segments; j++)
				{
					float num10 = (float)j / (float)segments;
					float f;
					if (flag)
					{
						float num11 = num5 - num4;
						if (num11 > 0f)
						{
							num11 -= MathF.PI * 2f;
						}
						f = num4 + num10 * num11;
					}
					else
					{
						float num12 = num5 - num4;
						if (num12 < 0f)
						{
							num12 += MathF.PI * 2f;
						}
						f = num4 + num10 * num12;
					}
					array[j] = center + new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * radius;
				}
			}
			else
			{
				for (int k = 0; k <= segments; k++)
				{
					float t2 = (float)k / (float)segments;
					array[k] = QuadraticBezier(fromPos, vector4, toPos, t2);
				}
			}
		}
		return array;
	}

	private bool CalculateCircleFromThreePoints(Vector2 p1, Vector2 p2, Vector2 p3, out Vector2 center, out float radius)
	{
		center = Vector2.zero;
		radius = 0f;
		Vector2 vector = (p1 + p2) * 0.5f;
		Vector2 vector2 = (p2 + p3) * 0.5f;
		Vector2 normalized = (p2 - p1).normalized;
		Vector2 normalized2 = (p3 - p2).normalized;
		Vector2 vector3 = new Vector2(0f - normalized.y, normalized.x);
		Vector2 vector4 = new Vector2(0f - normalized2.y, normalized2.x);
		float num = vector3.x * vector4.y - vector3.y * vector4.x;
		if (Mathf.Abs(num) < 0.001f)
		{
			return false;
		}
		Vector2 vector5 = vector2 - vector;
		float num2 = (vector5.x * vector4.y - vector5.y * vector4.x) / num;
		center = vector + num2 * vector3;
		radius = Vector2.Distance(center, p1);
		return true;
	}

	private float NormalizeAngle(float angle)
	{
		while (angle > MathF.PI)
		{
			angle -= MathF.PI * 2f;
		}
		while (angle < -MathF.PI)
		{
			angle += MathF.PI * 2f;
		}
		return angle;
	}

	private Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
	{
		float num = 1f - t;
		return num * num * p0 + 2f * num * t * p1 + t * t * p2;
	}

	private void ClearLineSegments()
	{
		SaveLineSegmentsToSerialization();
		if (this == null || Equals(null))
		{
			UnityEngine.Debug.LogWarning("UILineRenderer: Component is being destroyed, attempting to clear segments anyway");
		}
		for (int num = lineSegments.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = lineSegments[num];
			if (gameObject != null)
			{
				if (gameObject.transform.parent == base.transform)
				{
					try
					{
						if (Application.isPlaying)
						{
							UnityEngine.Object.Destroy(gameObject);
						}
						else
						{
							UnityEngine.Object.DestroyImmediate(gameObject);
						}
					}
					catch (Exception ex)
					{
						DebugLogError("UILineRenderer: Error destroying line segment: " + ex.Message);
					}
				}
				else if (gameObject.transform.parent != null)
				{
					UnityEngine.Debug.LogWarning("UILineRenderer: Segment '" + gameObject.name + "' belongs to different parent '" + gameObject.transform.parent.name + "', removing from list without destroying");
				}
			}
		}
		lineSegments.Clear();
		if (this != null && !Equals(null) && base.transform != null)
		{
			try
			{
				CleanupOrphanedSegments();
			}
			catch (Exception ex2)
			{
				DebugLogError("UILineRenderer: Error during orphaned segment cleanup: " + ex2.Message);
			}
		}
	}

	private void CleanupOrphanedSegments()
	{
		if (base.transform == null)
		{
			return;
		}
		int num = 0;
		for (int num2 = lineSegments.Count - 1; num2 >= 0; num2--)
		{
			GameObject gameObject = lineSegments[num2];
			if (gameObject == null)
			{
				lineSegments.RemoveAt(num2);
			}
			else if (gameObject.transform.parent != base.transform)
			{
				UnityEngine.Debug.LogWarning("UILineRenderer: Found orphaned segment '" + gameObject.name + "' that was moved from parent, removing from list");
				lineSegments.RemoveAt(num2);
				num++;
			}
		}
	}

	private void CreateLineSegments(Vector2[] points)
	{
		bool flag = false;
		if (lineSegments.Count != points.Length - 1)
		{
			flag = true;
		}
		else if (!ValidateSegments())
		{
			flag = true;
		}
		if (flag)
		{
			ClearLineSegments();
			if (base.transform == null || parentCanvas == null)
			{
				UnityEngine.Debug.LogWarning("UILineRenderer: Cannot create line segments - invalid transform or parent canvas");
				return;
			}
			if (!base.gameObject.activeInHierarchy || !base.enabled)
			{
				UnityEngine.Debug.LogWarning("UILineRenderer: Cannot create line segments - component is not active or enabled");
				return;
			}
			if (lineSegmentPrefab == null)
			{
				CreateDefaultLineSegmentPrefab();
				if (lineSegmentPrefab == null)
				{
					DebugLogError("UILineRenderer: Failed to create default lineSegmentPrefab, cannot proceed");
					return;
				}
			}
			if (lineSegmentPrefab.Equals(null))
			{
				DebugLogError("UILineRenderer: lineSegmentPrefab has been destroyed, cannot create segments");
				return;
			}
			for (int i = 0; i < points.Length - 1; i++)
			{
				Vector2 vector = points[i];
				Vector2 vector2 = points[i + 1];
				GameObject gameObject = null;
				try
				{
					if (lineSegmentPrefab == null || lineSegmentPrefab.Equals(null))
					{
						DebugLogError("UILineRenderer: lineSegmentPrefab is null/destroyed during instantiation");
						continue;
					}
					if (base.transform == null || base.transform.Equals(null))
					{
						DebugLogError("UILineRenderer: transform is null/destroyed during instantiation");
						continue;
					}
					gameObject = UnityEngine.Object.Instantiate(lineSegmentPrefab, base.transform);
					if (!gameObject.activeInHierarchy)
					{
						gameObject.SetActive(value: true);
					}
					if (gameObject.transform.parent != base.transform)
					{
						UnityEngine.Debug.LogWarning("UILineRenderer: Segment was not properly parented, fixing...");
						gameObject.transform.SetParent(base.transform, worldPositionStays: false);
					}
				}
				catch (Exception ex)
				{
					DebugLogError("UILineRenderer: Failed to create line segment: " + ex.Message);
					if (gameObject != null)
					{
						UnityEngine.Object.DestroyImmediate(gameObject);
					}
					continue;
				}
				if (gameObject == null)
				{
					DebugLogError("UILineRenderer: Failed to instantiate line segment");
					continue;
				}
				RectTransform component = gameObject.GetComponent<RectTransform>();
				if (component == null)
				{
					DebugLogError("UILineRenderer: Line segment missing RectTransform component");
					UnityEngine.Object.DestroyImmediate(gameObject);
					continue;
				}
				Vector2 anchoredPosition = (vector + vector2) * 0.5f;
				component.anchoredPosition = anchoredPosition;
				Vector2 vector3 = vector2 - vector;
				float angle = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
				component.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				float x = Vector2.Distance(vector, vector2);
				component.sizeDelta = new Vector2(x, lineWidth);
				Image component2 = gameObject.GetComponent<Image>();
				if (component2 != null)
				{
					component2.color = CurrentLinkColor;
					if (linkSprite != null)
					{
						component2.sprite = linkSprite;
					}
					if (lineMaterial != null)
					{
						component2.material = lineMaterial;
					}
				}
				lineSegments.Add(gameObject);
			}
			SaveLineSegmentsToSerialization();
		}
		else
		{
			UpdateExistingSegments(points);
		}
	}

	public void CleanupLineSegments()
	{
		ClearLineSegments();
		CleanupAllOrphanedSegments();
	}

	private void CleanupAllOrphanedSegments()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>(includeInactive: true);
		int num = 0;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!gameObject.name.Contains("LineSegment") || (!(gameObject.transform.parent == null) && !gameObject.transform.parent.name.Contains("Canvas") && !(gameObject.transform.parent == gameObject.transform.root)))
			{
				continue;
			}
			UnityEngine.Debug.LogWarning("UILineRenderer: Found orphaned LineSegment '" + gameObject.name + "' during global cleanup, destroying it");
			try
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
				num++;
			}
			catch (Exception ex)
			{
				DebugLogError("UILineRenderer: Error destroying orphaned segment during global cleanup: " + ex.Message);
			}
		}
	}

	public static void CleanupAllTemplatePrefabs()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>(includeInactive: true);
		int num = 0;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!gameObject.name.Contains("LineSegment_Template") && (!gameObject.name.Contains("Template") || !gameObject.name.Contains("LineSegment")))
			{
				continue;
			}
			bool flag = false;
			string text = "";
			if (gameObject.transform.parent == null)
			{
				flag = true;
				text = "top level (old system)";
			}
			else if (gameObject.transform.parent.name.Contains("Canvas"))
			{
				flag = true;
				text = "direct child of Canvas (old system)";
			}
			else if (gameObject.transform.parent != null)
			{
				UILineRenderer component = gameObject.transform.parent.GetComponent<UILineRenderer>();
				if (component == null)
				{
					flag = true;
					text = "orphaned from UILineRenderer";
				}
				else if (component.lineSegmentPrefab != gameObject)
				{
					flag = true;
					text = "not current template of parent UILineRenderer";
				}
			}
			if (!flag)
			{
				continue;
			}
			UnityEngine.Debug.LogWarning("UILineRenderer: Found orphaned template prefab '" + gameObject.name + "' (" + text + "), destroying it");
			try
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
				num++;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("UILineRenderer: Error destroying orphaned template prefab: " + ex.Message);
			}
		}
	}

	public void ConfigureLinkVisuals(Sprite sprite, Color unlockedColor, Color lockedColor, bool locked = false)
	{
		linkSprite = sprite;
		unlockedLinkColor = unlockedColor;
		lockedLinkColor = lockedColor;
		isLocked = locked;
		UpdateLineVisuals();
	}

	private bool ValidateSegments()
	{
		if (base.transform == null || lineSegments == null)
		{
			return false;
		}
		int num = 0;
		for (int num2 = lineSegments.Count - 1; num2 >= 0; num2--)
		{
			GameObject gameObject = lineSegments[num2];
			if (gameObject != null && gameObject.transform.parent == base.transform)
			{
				num++;
			}
		}
		return num > 0;
	}

	private void RestoreLineSegmentsFromSerialization()
	{
		bool flag = serializedFromNode != null && !serializedFromNode.Equals(null) && serializedToNode != null && !serializedToNode.Equals(null);
		if (serializedLineSegments.Count > 0 && lineSegments.Count == 0)
		{
			for (int num = serializedLineSegments.Count - 1; num >= 0; num--)
			{
				if (serializedLineSegments[num] == null)
				{
					serializedLineSegments.RemoveAt(num);
				}
			}
			lineSegments.Clear();
			lineSegments.AddRange(serializedLineSegments);
		}
		if (!flag)
		{
			return;
		}
		if (serializedFromNode.Equals(null) || serializedToNode.Equals(null))
		{
			DebugLogError("UILineRenderer: Cannot restore - serialized fromNode or toNode has been destroyed");
			serializedFromNode = null;
			serializedToNode = null;
			serializedCurveAmount = 0f;
			return;
		}
		fromNode = serializedFromNode;
		toNode = serializedToNode;
		curveAmount = serializedCurveAmount;
		if (rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform>();
		}
		EnsureParentCanvas();
		if (lineSegments.Count == 0 && fromNode != null && toNode != null)
		{
			CreateLine();
		}
		else if (lineSegments.Count > 0)
		{
			if (!ValidateSegments())
			{
				ClearLineSegments();
				CreateLine();
			}
			else
			{
				Vector2 canvasPosition = GetCanvasPosition(fromNode);
				Vector2 canvasPosition2 = GetCanvasPosition(toNode);
				Vector2[] points = GenerateLinePoints(canvasPosition, canvasPosition2, curveAmount, segmentCount);
				UpdateExistingSegments(points);
			}
		}
		serializedLineSegments.Clear();
	}

	private void SaveLineSegmentsToSerialization()
	{
		serializedLineSegments.Clear();
		foreach (GameObject lineSegment in lineSegments)
		{
			if (lineSegment != null)
			{
				serializedLineSegments.Add(lineSegment);
			}
		}
		serializedFromNode = fromNode;
		serializedToNode = toNode;
		serializedCurveAmount = curveAmount;
	}

	private void UpdateExistingSegments(Vector2[] points)
	{
		if (lineSegments == null || points == null || lineSegments.Count != points.Length - 1)
		{
			UnityEngine.Debug.LogWarning("UILineRenderer: Cannot update existing segments - invalid state");
			return;
		}
		for (int i = 0; i < points.Length - 1; i++)
		{
			Vector2 vector = points[i];
			Vector2 vector2 = points[i + 1];
			if (i >= lineSegments.Count)
			{
				break;
			}
			GameObject gameObject = lineSegments[i];
			if (gameObject == null)
			{
				continue;
			}
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if (component == null)
			{
				continue;
			}
			Vector2 anchoredPosition = (vector + vector2) * 0.5f;
			component.anchoredPosition = anchoredPosition;
			Vector2 vector3 = vector2 - vector;
			float angle = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			component.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			float x = Vector2.Distance(vector, vector2);
			component.sizeDelta = new Vector2(x, lineWidth);
			Image component2 = gameObject.GetComponent<Image>();
			if (component2 != null)
			{
				component2.color = CurrentLinkColor;
				if (linkSprite != null)
				{
					component2.sprite = linkSprite;
				}
				if (lineMaterial != null)
				{
					component2.material = lineMaterial;
				}
			}
		}
		SaveLineSegmentsToSerialization();
	}

	private void UpdateLineVisuals()
	{
		if (lineSegments == null || lineSegments.Count == 0)
		{
			return;
		}
		Color currentLinkColor = CurrentLinkColor;
		foreach (GameObject lineSegment in lineSegments)
		{
			if (!(lineSegment != null))
			{
				continue;
			}
			Image component = lineSegment.GetComponent<Image>();
			if (component != null)
			{
				component.color = currentLinkColor;
				if (linkSprite != null)
				{
					component.sprite = linkSprite;
				}
			}
		}
	}

	[ContextMenu("Force Sync")]
	public void ForceSync()
	{
		if (this == null || Equals(null))
		{
			DebugLogError("UILineRenderer: Cannot force sync - component has been destroyed");
			return;
		}
		if (!base.gameObject.activeInHierarchy || !base.enabled)
		{
			UnityEngine.Debug.LogWarning("UILineRenderer: Cannot force sync - component is not active or enabled");
			return;
		}
		if (rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform>();
		}
		if (EnsureParentCanvas() == null)
		{
			DebugLogError("UILineRenderer: Cannot force sync - no parent canvas found");
		}
		else if (rectTransform == null)
		{
			DebugLogError("UILineRenderer: Cannot force sync - RectTransform component is missing!");
		}
		else if (fromNode != null && toNode != null)
		{
			if (fromNode.Equals(null) || toNode.Equals(null))
			{
				DebugLogError("UILineRenderer: Cannot force sync - fromNode or toNode has been destroyed");
			}
			else
			{
				CreateLine();
			}
		}
		else if (serializedFromNode != null && serializedToNode != null)
		{
			if (serializedFromNode.Equals(null) || serializedToNode.Equals(null))
			{
				DebugLogError("UILineRenderer: Cannot force sync - serialized fromNode or toNode has been destroyed");
				return;
			}
			fromNode = serializedFromNode;
			toNode = serializedToNode;
			curveAmount = serializedCurveAmount;
			CreateLine();
		}
		else
		{
			UnityEngine.Debug.LogWarning("UILineRenderer: Cannot force sync on " + base.gameObject.name + " - no valid node references found (fromNode: " + ((fromNode != null) ? fromNode.name : "null") + ", toNode: " + ((toNode != null) ? toNode.name : "null") + ")");
		}
	}
}
[Serializable]
public class BarSelfer
{
	public Image BarImage;

	public TextMeshProUGUI BarText;

	private double percentage;

	public BarTextFormat barTextFormat;

	public string format;

	public string suffix;

	public bool isCountAsNormalDouble;

	public BarSelfer(Image barImage, TextMeshProUGUI barText, BarTextFormat barTextFormat, string format, string suffix)
	{
		BarImage = barImage;
		BarText = barText;
		this.barTextFormat = barTextFormat;
		this.format = format;
		this.suffix = suffix;
	}

	public void ManageBar(double currentValue, double maxValue)
	{
		percentage = currentValue / maxValue;
		BarImage.fillAmount = (float)percentage;
		if (!(BarText != null))
		{
			return;
		}
		if (barTextFormat == BarTextFormat.Current_Max)
		{
			if (format == "")
			{
				BarText.text = currentValue.ToReadable() + " / " + maxValue.ToReadable();
			}
			else
			{
				BarText.text = currentValue.ToString(format) + " / " + maxValue.ToString(format);
			}
		}
		else if (barTextFormat == BarTextFormat.Current)
		{
			if (format == "")
			{
				BarText.text = currentValue.ToReadable();
			}
			else
			{
				BarText.text = (isCountAsNormalDouble ? (currentValue.ToString(format) + suffix) : (currentValue.ToString(format) + suffix));
			}
		}
		else if (barTextFormat == BarTextFormat.Percentage)
		{
			BarText.text = (currentValue / maxValue * 100.0).ToString(format) + "%";
		}
		else if (barTextFormat == BarTextFormat.MaxMinusCurrent)
		{
			BarText.text = (maxValue - currentValue).ToString(format);
		}
	}
}
public enum BarTextFormat
{
	Current_Max,
	Current,
	Percentage,
	MaxMinusCurrent
}
public class Breather : MonoBehaviour
{
	public Transform Trans_NullForSelf;

	public float scaleFactor = 1.2f;

	public float duration = 1f;

	private Vector3 originalScale;

	public bool isPopOnStart = true;

	public bool isBreathingOnCall;

	public float popScaleFactor = 1.2f;

	public float popDuration = 0.5f;

	private Tween breathingTween;

	public bool isBreathAfterPop = true;

	private bool isInit;

	private void Init()
	{
		if (!isInit)
		{
			isInit = true;
			if (Trans_NullForSelf == null)
			{
				Trans_NullForSelf = base.transform;
			}
			originalScale = Trans_NullForSelf.localScale;
		}
	}

	private void OnEnable()
	{
		Init();
		if (isPopOnStart)
		{
			Pop();
		}
		else if (!isBreathingOnCall)
		{
			StartBreathing();
		}
	}

	private void Start()
	{
		Init();
		if (isPopOnStart)
		{
			Pop();
		}
		else if (!isBreathingOnCall)
		{
			StartBreathing();
		}
	}

	public void SetSizeToZero()
	{
		Init();
		Trans_NullForSelf.localScale = Vector3.zero;
	}

	public void Pop()
	{
		Init();
		DOTween.Kill(base.gameObject.GetHashCode());
		Trans_NullForSelf.localScale = Vector3.zero;
		DG.Tweening.Sequence sequence = DOTween.Sequence().SetId(base.gameObject.GetHashCode());
		sequence.Append(Trans_NullForSelf.DOScale(originalScale * popScaleFactor, popDuration * 0.75f).SetEase(Ease.OutBack));
		sequence.Append(Trans_NullForSelf.DOScale(originalScale, popDuration * 0.75f).SetEase(Ease.OutBack));
		sequence.OnComplete(delegate
		{
			if (isBreathAfterPop)
			{
				StartBreathing();
			}
		});
	}

	public void StopBreathing()
	{
		Init();
		breathingTween?.Kill();
		DOTween.Kill(base.gameObject.GetHashCode());
		Trans_NullForSelf.localScale = originalScale;
	}

	public void StartBreathing()
	{
		Init();
		breathingTween = Trans_NullForSelf.DOScale(originalScale * scaleFactor, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
			.SetId(base.gameObject.GetHashCode());
	}
}
public class ButtonPressedMoveTextDown : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public Transform textToMove;

	public float YMoveAmount = -7f;

	private Vector3 originalPosition;

	private Button myButton;

	private void Start()
	{
		originalPosition = textToMove.localPosition;
		myButton = GetComponent<Button>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (myButton.interactable && textToMove != null)
		{
			textToMove.localPosition = new Vector3(textToMove.localPosition.x, originalPosition.y + YMoveAmount, textToMove.localPosition.z);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (myButton.interactable && textToMove != null)
		{
			textToMove.localPosition = originalPosition;
		}
	}
}
public class CustomAnimator : MonoBehaviour
{
	private Vector3 originalScale;

	public Transform Trans_NullForSelf;

	public bool isBreath;

	public float breathScaleFactor = 1.08f;

	public float breathDuration = 0.8f;

	public bool isPopOnStart;

	public float popScaleFactor = 1.2f;

	public float popDuration = 0.5f;

	public float inversePopDuration = 0.5f;

	private Tween breathingTween;

	private bool isInit;

	private bool isFinishedPoppingIn;

	private void OnEnable()
	{
		if (!isInit)
		{
			isInit = true;
			if (Trans_NullForSelf == null)
			{
				Trans_NullForSelf = base.transform;
			}
			originalScale = Trans_NullForSelf.localScale;
			breathDuration = UnityEngine.Random.Range(breathDuration * 0.95f, breathDuration * 1.05f);
		}
		Trans_NullForSelf.localScale = originalScale;
		isFinishedPoppingIn = true;
		if (isPopOnStart)
		{
			isFinishedPoppingIn = false;
			Pop(isStartFromZeroScale: true);
		}
		else if (isBreath)
		{
			StartBreathing();
		}
	}

	public void Pop(bool isStartFromZeroScale = false)
	{
		breathingTween?.Pause();
		if (isStartFromZeroScale)
		{
			Trans_NullForSelf.localScale = Vector3.zero;
		}
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.Append(Trans_NullForSelf.DOScale(originalScale * popScaleFactor, popDuration * 0.75f).SetEase(Ease.OutBack));
		sequence.Append(Trans_NullForSelf.DOScale(originalScale, popDuration * 0.5f).SetEase(Ease.OutBack));
		if (!isBreath)
		{
			return;
		}
		if (breathingTween == null)
		{
			sequence.OnComplete(delegate
			{
				StartBreathing();
			}).OnComplete(delegate
			{
				isFinishedPoppingIn = true;
			});
		}
		else
		{
			sequence.OnComplete(delegate
			{
				breathingTween.Play();
			}).OnComplete(delegate
			{
				isFinishedPoppingIn = true;
			});
		}
	}

	public void InversePop(bool isDisableOnComplete, GameObject toDisable)
	{
		if (!isFinishedPoppingIn)
		{
			toDisable.SetActive(value: false);
			return;
		}
		breathingTween?.Pause();
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		sequence.Append(Trans_NullForSelf.DOScale(0f, inversePopDuration).SetEase(Ease.OutBack));
		if (isDisableOnComplete)
		{
			sequence.OnComplete(delegate
			{
				toDisable.SetActive(value: false);
			});
		}
	}

	private void StartBreathing()
	{
		breathingTween?.Kill();
		UnityEngine.Debug.Log(originalScale * breathScaleFactor);
		breathingTween = Trans_NullForSelf.DOScale(originalScale * breathScaleFactor, breathDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
	}
}
public class HoverAnimator : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Transform AnimatedTrans;

	[SerializeField]
	private float scaleOnHover = 1.15f;

	[SerializeField]
	private float scaleTransition = 0.15f;

	[SerializeField]
	private Ease scaleEase = Ease.OutBack;

	public bool isChangeColor;

	public Image AnimatedImage;

	public Color originalColor;

	public Color hoverColor;

	public bool isBreathe;

	public float scaleFactor = 1.2f;

	public float duration = 1f;

	private Tween breathingTween;

	private void Start()
	{
		if (AnimatedTrans == null)
		{
			AnimatedTrans = base.transform;
		}
		if (AnimatedImage == null)
		{
			AnimatedImage = GetComponent<Image>();
		}
		if (isBreathe)
		{
			duration = UnityEngine.Random.Range(duration * 0.95f, duration * 1.05f);
			StartBreathing();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		breathingTween?.Pause();
		AnimatedTrans.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
		if (isChangeColor)
		{
			AnimatedImage.color = hoverColor;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		AnimatedTrans.DOScale(1f, scaleTransition).SetEase(scaleEase).OnComplete(delegate
		{
			breathingTween?.Play();
		});
		if (isChangeColor)
		{
			AnimatedImage.color = originalColor;
		}
	}

	private void StartBreathing()
	{
		Vector3 localScale = AnimatedTrans.localScale;
		breathingTween = AnimatedTrans.DOScale(localScale * scaleFactor, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
	}
}
public class MainMenusManager : SerializedMonoBehaviour
{
	public static MainMenusManager instance;

	public GameObject ButtonsParent;

	public Dictionary<UnlockableSystems, Button> MenuButtons;

	public Dictionary<UnlockableSystems, Transform> MenuTransforms;

	private Dictionary<UnlockableSystems, Image> MenuImages;

	private Dictionary<UnlockableSystems, GameObject> LockIconGameObjects;

	private Dictionary<UnlockableSystems, GameObject> NormalIconGameObjects;

	private Dictionary<UnlockableSystems, GameObject> NotificationGameObjects;

	public Color ButtonNormalColor;

	public Color ButtonHighlightedColor;

	public Color DisabledButtonColor;

	public Vector2 ShowPosition;

	public Vector2 HidePosition;

	private int CurrentClickedSystem;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		CurrentClickedSystem = -1;
		MenuImages = new Dictionary<UnlockableSystems, Image>();
		LockIconGameObjects = new Dictionary<UnlockableSystems, GameObject>();
		NormalIconGameObjects = new Dictionary<UnlockableSystems, GameObject>();
		NotificationGameObjects = new Dictionary<UnlockableSystems, GameObject>();
		foreach (KeyValuePair<UnlockableSystems, Button> menuButton in MenuButtons)
		{
			MenuImages.Add(menuButton.Key, menuButton.Value.GetComponent<Image>());
			LockIconGameObjects.Add(menuButton.Key, menuButton.Value.transform.Find("LockIcon").gameObject);
			NormalIconGameObjects.Add(menuButton.Key, menuButton.Value.transform.Find("MainIcon").gameObject);
			NotificationGameObjects.Add(menuButton.Key, menuButton.Value.transform.Find("Notification").gameObject);
			NotificationGameObjects[menuButton.Key].SetActive(value: false);
		}
		CheckButtons();
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Tree])
		{
			ClickedOnButton(0);
		}
	}

	public void CheckButtons()
	{
		bool flag = false;
		foreach (KeyValuePair<UnlockableSystems, Button> menuButton in MenuButtons)
		{
			menuButton.Value.interactable = playerData.instance.UnlockedSystems[menuButton.Key];
			MenuImages[menuButton.Key].color = (playerData.instance.UnlockedSystems[menuButton.Key] ? ButtonNormalColor : DisabledButtonColor);
			LockIconGameObjects[menuButton.Key].SetActive(!playerData.instance.UnlockedSystems[menuButton.Key]);
			NormalIconGameObjects[menuButton.Key].SetActive(playerData.instance.UnlockedSystems[menuButton.Key]);
			if (playerData.instance.UnlockedSystems[menuButton.Key])
			{
				flag = true;
			}
		}
		ButtonsParent.SetActive(flag);
		TutorialManager.instance.CheckButtons();
		if (flag && CurrentClickedSystem == -1)
		{
			CurrentClickedSystem = 0;
			ClickedOnButton(0);
		}
		else if (CurrentClickedSystem != -1)
		{
			ClickedOnButton(CurrentClickedSystem);
		}
	}

	public void ClickedOnButton(int system)
	{
		foreach (KeyValuePair<UnlockableSystems, Image> menuImage in MenuImages)
		{
			menuImage.Value.color = (playerData.instance.UnlockedSystems[menuImage.Key] ? ButtonNormalColor : DisabledButtonColor);
			MenuTransforms[menuImage.Key].position = HidePosition;
		}
		if (!MenuTransforms[(UnlockableSystems)system].gameObject.activeInHierarchy)
		{
			MenuTransforms[(UnlockableSystems)system].gameObject.SetActive(value: true);
		}
		MenuImages[(UnlockableSystems)system].color = ButtonHighlightedColor;
		MenuTransforms[(UnlockableSystems)system].position = ShowPosition;
		CurrentClickedSystem = system;
		ShowHideNotificationInSystem((UnlockableSystems)system, isShow: false);
		TutorialManager.instance.ClickedOnButton(system);
		TutorialManager.instance.ForceShowHideTutorial(isShow: false);
		if (system == 4)
		{
			CharacterUIManager.instance.UpdateDamageText();
		}
		if (system == 8 && playerData.instance.WellFillCount >= DatabaseManager.MaxWellFillCount && !playerData.instance.isUnlockedBosses)
		{
			WellManager.instance.ClickedOnWellPower(8);
		}
	}

	public void ShowHideNotificationInSystem(UnlockableSystems system, bool isShow)
	{
		if (!isShow || CurrentClickedSystem != (int)system)
		{
			NotificationGameObjects[system].SetActive(isShow);
		}
	}
}
public enum UnlockableSystems
{
	Tree,
	Skills,
	Shiny,
	Items,
	Heroes,
	Gems,
	Bounties,
	TreasureChest,
	Well,
	Towers
}
public class OnHoverUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public GameObject ThingToEnableDisable;

	public UnityEvent onHoverEnter;

	public UnityEvent onHoverExit;

	public bool isScaleOnHover;

	public Transform Trans_NullForSelf_Scale;

	public float scaleOnHover = 1.05f;

	public float scaleDuration = 1f;

	public bool isBreatheOnHover;

	private Tween scaleTween;

	private Vector3 originalScale;

	private Breather breather;

	private bool isInit;

	public void Init()
	{
		if (!isInit)
		{
			if (ThingToEnableDisable != null)
			{
				ThingToEnableDisable.SetActive(value: false);
			}
			if (Trans_NullForSelf_Scale == null)
			{
				Trans_NullForSelf_Scale = base.transform;
			}
			originalScale = base.transform.localScale;
			breather = GetComponent<Breather>();
			isInit = true;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Init();
		if (ThingToEnableDisable != null)
		{
			ThingToEnableDisable.SetActive(value: true);
		}
		if (onHoverEnter != null)
		{
			onHoverEnter.Invoke();
		}
		if (breather != null)
		{
			breather.StopBreathing();
		}
		if (isScaleOnHover)
		{
			if (isBreatheOnHover)
			{
				scaleTween = Trans_NullForSelf_Scale.DOScale(originalScale * scaleOnHover, scaleDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			}
			else
			{
				scaleTween = Trans_NullForSelf_Scale.DOScale(originalScale * scaleOnHover, scaleDuration).SetEase(Ease.InOutSine);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Init();
		if (ThingToEnableDisable != null)
		{
			ThingToEnableDisable.SetActive(value: false);
		}
		if (onHoverExit != null)
		{
			onHoverExit.Invoke();
		}
		if (isScaleOnHover)
		{
			scaleTween?.Kill();
			Trans_NullForSelf_Scale.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutSine);
		}
	}
}
public class OnHoverUISound : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	private Button button;

	private void Start()
	{
		if (TryGetComponent<Button>(out var component))
		{
			button = component;
		}
		else
		{
			button = null;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!(button != null) || button.interactable)
		{
			FXManager.instance.PlayGeneralSound(GeneralSounds.generic_ui_hover);
		}
	}
}
public class SettingsManager : MonoBehaviour
{
	public static SettingsManager instance;

	public TMP_Dropdown ResolutionDD;

	public AudioMixer mixer;

	public GameObject FullScreenCheckmark;

	public Slider EffectsSoundSlider;

	public Slider UISoundSlider;

	public TextMeshProUGUI EffectsSoundSliderValue;

	public TextMeshProUGUI UISoundSliderValue;

	public TextMeshProUGUI VersionText;

	public Transform SettingsUI;

	public Slider EffectsAmountSlider;

	public TextMeshProUGUI EffectsAmountSliderValue;

	public GameObject DamageFloatingShowButton;

	public GameObject DamageFloatingHideButton;

	[HideInInspector]
	public bool isSettingsShown;

	public Transform MainMenuSettingsButtonTransform;

	public int WhichButtonCalledShowSettings;

	public Transform LanguagesMenu;

	public Transform LanguagesMenuButtonTransform;

	[HideInInspector]
	public bool isLanguagesMenuShown;

	public TextMeshProUGUI CurrentLanguageText;

	public TextMeshProUGUI QuitButtonText;

	private Dictionary<Languages, string> Languages_DisplayNames = new Dictionary<Languages, string>();

	private Languages LanguageStartedWithTheGame;

	public LocalizedTextObject LaunchGameLocalizedObject;

	private GameObject LaunchGameTextGo;

	private Vector2 LanguagesMenuOriginalPosition;

	private Ease easeType = Ease.InOutCubic;

	private float duration = 0.2f;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		ManageSettingsSliders("EffectsVolume");
		ManageSettingsSliders("UIVolume");
		ManageSettingsSliders("EffectsAmount");
		CheckResolution();
		ResolutionDDManager();
		LanguagesMenuOriginalPosition = LanguagesMenu.position - LanguagesMenuButtonTransform.position;
		isSettingsShown = false;
		Languages_DisplayNames.Add(Languages.English, "English");
		Languages_DisplayNames.Add(Languages.French, "Français");
		Languages_DisplayNames.Add(Languages.Spanish, "Español");
		Languages_DisplayNames.Add(Languages.German, "Deutsch");
		Languages_DisplayNames.Add(Languages.Italian, "Italiano");
		Languages_DisplayNames.Add(Languages.Portuguese, "Português");
		Languages_DisplayNames.Add(Languages.Russian, "Русский");
		Languages_DisplayNames.Add(Languages.Chinese, "中文");
		Languages_DisplayNames.Add(Languages.Japanese, "日本語");
		Languages_DisplayNames.Add(Languages.Korean, "한국어");
		Languages_DisplayNames.Add(Languages.Czech, "Čeština");
		Languages_DisplayNames.Add(Languages.Polish, "Polski");
		Languages_DisplayNames.Add(Languages.Turkish, "Türkçe");
		Languages_DisplayNames.Add(Languages.Ukrainian, "Українська");
		LaunchGameTextGo = LaunchGameLocalizedObject.gameObject;
		LanguageStartedWithTheGame = playerData.instance.ChoosenLanguage;
		ClickedOnLanguage((int)playerData.instance.ChoosenLanguage);
		VersionText.text = "v" + SaveLoadManager.CurrentVersion_Expansion + "." + SaveLoadManager.CurrentVersion_Major + "." + SaveLoadManager.CurrentVersion_Minor;
		ChangeFloatingNumbers(playerData.instance.isDamageFloatingShown);
	}

	public void ResolutionDDManager()
	{
		int resolutionChoice = playerData.instance.ResolutionChoice;
		resolutionChoice = ((resolutionChoice != 3) ? (resolutionChoice + 1) : 0);
		ResolutionDD.SetValueWithoutNotify(resolutionChoice);
		FullScreenCheckmark.SetActive(playerData.instance.IsFullScreen);
	}

	public void ClickedOnLanguage(int WhichLang)
	{
		LocalizerManager.ChangeLanguage(WhichLang);
		if (playerData.instance.ChoosenLanguage != LanguageStartedWithTheGame)
		{
			LaunchGameLocalizedObject.UpdateFromOutside();
			LaunchGameTextGo.SetActive(value: true);
		}
		else
		{
			LaunchGameTextGo.SetActive(value: false);
		}
		CurrentLanguageText.text = Languages_DisplayNames[playerData.instance.ChoosenLanguage];
	}

	public void ChangeResolution(int Reso)
	{
		FXManager.instance.PlayUIClickSound();
		Reso = ((Reso != 0) ? (Reso - 1) : 3);
		playerData.instance.ResolutionChoice = Reso;
		CheckResolution();
	}

	public void ChangeFullScreen()
	{
		playerData.instance.IsFullScreen = !playerData.instance.IsFullScreen;
		FullScreenCheckmark.SetActive(playerData.instance.IsFullScreen);
		CheckResolution();
	}

	public void CheckResolution()
	{
		if (playerData.instance.ResolutionChoice == 0)
		{
			Screen.SetResolution(1920, 1080, playerData.instance.IsFullScreen);
		}
		else if (playerData.instance.ResolutionChoice == 1)
		{
			Screen.SetResolution(1600, 900, playerData.instance.IsFullScreen);
		}
		else if (playerData.instance.ResolutionChoice == 2)
		{
			Screen.SetResolution(1280, 720, playerData.instance.IsFullScreen);
		}
		else if (playerData.instance.ResolutionChoice == 3)
		{
			Screen.SetResolution(2560, 1440, playerData.instance.IsFullScreen);
		}
	}

	public void SoundSlider_Effects(float SoundVolume)
	{
		if (SoundVolume <= 5f)
		{
			SoundVolume = 0f;
		}
		playerData.instance.SoundVolume_Effects = SoundVolume;
		SetGroupVolume("EffectsVolume", playerData.instance.SoundVolume_Effects);
		ManageSettingsSliders("EffectsVolume");
	}

	public void SoundSlider_UI(float SoundVolume)
	{
		if (SoundVolume <= 5f)
		{
			SoundVolume = 0f;
		}
		playerData.instance.SoundVolume_UI = SoundVolume;
		SetGroupVolume("UIVolume", playerData.instance.SoundVolume_UI);
		ManageSettingsSliders("UIVolume");
	}

	private void SetGroupVolume(string parameterName, float volumePercent)
	{
		float num = Mathf.Clamp01(volumePercent / 100f);
		float value = ((num <= 0f) ? (-80f) : (20f * Mathf.Log10(num)));
		mixer.SetFloat(parameterName, value);
	}

	public void ChangeFloatingNumbers(bool isShow)
	{
		playerData.instance.isDamageFloatingShown = isShow;
		DamageFloatingShowButton.SetActive(playerData.instance.isDamageFloatingShown);
		DamageFloatingHideButton.SetActive(!playerData.instance.isDamageFloatingShown);
	}

	public void EffectsSlider(float EffectsAmount)
	{
		playerData.instance.EffectsAmount = EffectsAmount;
		ManageSettingsSliders("EffectsAmount");
	}

	public void ManageSettingsSliders(string whichSlider)
	{
		switch (whichSlider)
		{
		case "EffectsVolume":
			EffectsSoundSliderValue.text = playerData.instance.SoundVolume_Effects.ToString("0") + "%";
			EffectsSoundSlider.SetValueWithoutNotify(playerData.instance.SoundVolume_Effects);
			break;
		case "UIVolume":
			UISoundSliderValue.text = playerData.instance.SoundVolume_UI.ToString("0") + "%";
			UISoundSlider.SetValueWithoutNotify(playerData.instance.SoundVolume_UI);
			break;
		case "EffectsAmount":
			EffectsAmountSliderValue.text = playerData.instance.EffectsAmount.ToString("0") + "%";
			EffectsAmountSlider.SetValueWithoutNotify(playerData.instance.EffectsAmount);
			break;
		}
	}

	public void ForceHideSettings()
	{
		if (isSettingsShown)
		{
			ShowHideSettings(0);
		}
	}

	public void ShowHideSettings(int WhichButtonCalledIt)
	{
		WhichButtonCalledShowSettings = WhichButtonCalledIt;
		ShowHideSettings();
	}

	public void ShowHideSettings()
	{
		if (isLanguagesMenuShown)
		{
			ShowHideLanguagesMenu();
		}
		isSettingsShown = !isSettingsShown;
		if (isSettingsShown && SettingsUI.transform.position.x < 1000f)
		{
			isSettingsShown = false;
		}
		if (isSettingsShown)
		{
			SettingsUI.transform.localScale = Vector2.zero;
			SettingsUI.transform.position = ((WhichButtonCalledShowSettings == 0) ? ((Vector3)Vector2.zero) : MainMenuSettingsButtonTransform.position);
			SettingsUI.transform.DOMove(Vector2.zero, duration).SetEase(easeType);
			SettingsUI.transform.DOScale(Vector2.one, duration).SetEase(easeType);
		}
		else
		{
			SettingsUI.transform.DOMove((WhichButtonCalledShowSettings == 0) ? ((Vector3)Vector2.zero) : MainMenuSettingsButtonTransform.position, duration).SetEase(easeType);
			SettingsUI.transform.DOScale(Vector2.zero, duration).SetEase(easeType).OnComplete(delegate
			{
				SettingsUI.transform.position = new Vector2(3000f, 3000f);
			});
		}
		if (RunManager.instance.isRunStarted)
		{
			QuitButtonText.text = LocalizerManager.GetTranslatedValue("AbandonRun_Text");
		}
		else
		{
			QuitButtonText.text = LocalizerManager.GetTranslatedValue("QuitGame_Text");
		}
	}

	public void QuitGameOrAbandonRun()
	{
		if (RunManager.instance.isRunStarted)
		{
			ShowHideSettings();
			AbandonRun();
		}
		else
		{
			QuitGame();
		}
	}

	public void AbandonRun()
	{
		ShowHideSettings();
		RunManager.instance.EndRun(isExitedRun: true);
		UIManager.instance.ClickedOnGoToMainMenu();
	}

	public void QuitGame()
	{
		ManagerOfTheGame.instance.QuitGame();
	}

	public void ShowHideLanguagesMenu()
	{
		isLanguagesMenuShown = !isLanguagesMenuShown;
		AnimateUIElement(isLanguagesMenuShown, LanguagesMenu, LanguagesMenuButtonTransform.position, LanguagesMenuButtonTransform.position + (Vector3)LanguagesMenuOriginalPosition);
	}

	private void AnimateUIElement(bool show, Transform element, Vector2 startPosition, Vector2 endPosition)
	{
		if (show)
		{
			element.gameObject.SetActive(value: true);
			element.transform.localScale = Vector2.zero;
			element.transform.position = startPosition;
			element.transform.DOMove(endPosition, duration).SetEase(easeType);
			element.transform.DOScale(Vector2.one, duration).SetEase(easeType);
		}
		else
		{
			element.transform.DOMove(startPosition, duration).SetEase(easeType);
			element.transform.DOScale(Vector2.zero, duration).SetEase(easeType).OnComplete(delegate
			{
				element.gameObject.SetActive(value: false);
			});
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			FXManager.instance.PlayUIClickSound();
			ShowHideSettings(0);
		}
	}
}
public class StatsViewManager : MonoBehaviour
{
	public static StatsViewManager instance;

	public TextMeshProUGUI StatsText;

	public RectTransform ContentParent;

	public Transform StatsViewPanel;

	public Transform ShowHideStatsViewButton;

	public Vector2 ShowPosition;

	private bool isStatsViewShown;

	public StatsFloat DamageMultiplier_PerTotalShinyFound = new StatsFloat();

	public StatsFloat DamageMultiplier_PerStatsInAllItemsEquipped = new StatsFloat();

	public StatsFloat DamageMultiplier_PerLevelOfGems = new StatsFloat();

	public StatsFloat DamageMultiplier_PerSecondInTimer = new StatsFloat();

	public StatsFloat DamageMultiplier_PerMonster = new StatsFloat();

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		UpdateStatsView();
	}

	public void ClickedOnShowHideStatsView()
	{
		if (!StatsViewPanel.gameObject.activeInHierarchy)
		{
			StatsViewPanel.gameObject.SetActive(value: true);
		}
		isStatsViewShown = !isStatsViewShown;
		FunctionsNeeded.AnimateUIElement(isStatsViewShown, StatsViewPanel, ShowHideStatsViewButton.position, ShowPosition);
		UpdateStatsView();
	}

	public void UpdateStatsView()
	{
		string text = "";
		double num = 1.0;
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerTotalShinyFound.Total.RealValue * (float)playerData.instance.TotalShinyFound / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerStatsInAllItemsEquipped.Total.RealValue * (float)playerData.instance.TotalStatsInItemsEquipped / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerBountyFound.Total.RealValue * (float)playerData.instance.TotalBountiesFound_CurrentRun / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerLevelOfGems.Total.RealValue * (float)playerData.instance.TotalGemsLeveledUp / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerSecondInTimer.Total.RealValue * playerData.instance.stats.Timer.Total.RealValue / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerAreaMarkApplied.Total.RealValue * (float)playerData.instance.TotalAreaMarksApplied_CurrentRun / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerMonster.Total.RealValue * (float)playerData.instance.stats.NumberOfMonsters.Total.RealValue / 100f);
		double number = playerData.instance.stats.Damage.Total.RealValue * num;
		text = text + LocalizerManager.GetTranslatedThenReplaceValues("DamageFlat", number.ToReadable()) + "\n";
		if (playerData.instance.stats.ChanceForTwiceHits.Total.RealValue > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceForTwiceHitsFlat", playerData.instance.stats.ChanceForTwiceHits.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.stats.ChanceForDoubleDamage.Total.RealValue > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceForDoubleDamageFlat", playerData.instance.stats.ChanceForDoubleDamage.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.stats.ChanceForTripleDamage.Total.RealValue > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceForTripleDamageFlat", playerData.instance.stats.ChanceForTripleDamage.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.stats.Timer.Total.RealValue > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("TimerFlat", playerData.instance.stats.Timer.Total.RealValue.ToString("0.00")) + "\n";
		}
		if (playerData.instance.stats.ChanceToFireMouseProjectile.Total.RealValue > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceToFireMouseProjectileFlat", playerData.instance.stats.ChanceToFireMouseProjectile.Total.RealValue.ToString("0.0")) + "\n";
		}
		if (playerData.instance.stats.MouseProjectile_Bounce.Total.RealValue > 0)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("MouseProjectile_BounceFlat", "+" + playerData.instance.stats.MouseProjectile_Bounce.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.stats.MouseProjectile_Pierce.Total.RealValue > 0)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("MouseProjectile_PierceFlat", playerData.instance.stats.MouseProjectile_Pierce.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.stats.MouseProjectile_Chain.Total.RealValue > 0)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("MouseProjectile_ChainFlat", playerData.instance.stats.MouseProjectile_Chain.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.stats.MouseProjectile_AdditionalProjectiles.Total.RealValue > 0)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("MouseProjectile_AdditionalProjectilesFlat", (1 + playerData.instance.stats.MouseProjectile_AdditionalProjectiles.Total.RealValue).ToString("0")) + "\n";
		}
		if (playerData.instance.stats.ChanceForMouseAttackToBeRaged.Total.RealValue > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceForMouseAttackToBeRagedFlat", playerData.instance.stats.ChanceForMouseAttackToBeRaged.Total.RealValue.ToString("0.0")) + "\n";
		}
		if (playerData.instance.stats.ChanceForIdleMouseAttackToBeRaged.Total.RealValue > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceForIdleMouseAttackToBeRagedFlat", playerData.instance.stats.ChanceForIdleMouseAttackToBeRaged.Total.RealValue.ToString("0.0")) + "\n";
		}
		if ((playerData.instance.stats.GoldGained.Total.RealValue - 1f) * 100f > 0f)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("GoldGainedAdditive", ((playerData.instance.stats.GoldGained.Total.RealValue - 1f) * 100f).ToString("0")) + "\n";
		}
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Items])
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ItemsRarityFlat", playerData.instance.stats.ItemsRarity.Total.RealValue.ToString("0")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ItemsChanceFlat", playerData.instance.stats.ItemsChance.Total.RealValue.ToString("0.0")) + "\n";
		}
		text = text + LocalizerManager.GetTranslatedThenReplaceValues("NumberOfMonstersFlat", playerData.instance.stats.NumberOfMonsters.Total.RealValue.ToString("0")) + "\n";
		if (playerData.instance.UnlockedSystems[UnlockableSystems.TreasureChest])
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("TreasureChanceFlat", playerData.instance.stats.TreasureChance.Total.RealValue.ToString("0.00")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("TreasureGoldMultiplierAdditive", ((playerData.instance.stats.TreasureGoldMultiplier.Total.RealValue - 1f) * 100f).ToString("0")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ExpGainedAdditive", ((playerData.instance.stats.ExpGained.Total.RealValue - 1f) * 100f).ToString("0")) + "\n";
		}
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Shiny])
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ShinyChanceFlat", playerData.instance.stats.ShinyChance.Total.RealValue.ToString("0.00")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ShinyRarityFlat", playerData.instance.stats.ShinyRarity.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Heroes])
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("GhostChanceFlat", playerData.instance.stats.GhostChance.Total.RealValue.ToString("0.00")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("CharacterCurrencyDropFlat", playerData.instance.stats.CharacterCurrencyDrop.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Gems])
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("OreChanceFlat", playerData.instance.stats.OreChance.Total.RealValue.ToString("0.00")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("OreChance_RichFlat", playerData.instance.stats.OreChance_Rich.Total.RealValue.ToString("0")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("OreDrop_GemCurrencyFlat", playerData.instance.stats.OreDrop_GemCurrency.Total.RealValue.ToString("0")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("RichOreMultiplierAdditive", ((playerData.instance.stats.RichOreMultiplier.Total.RealValue - 1f) * 100f).ToString("0")) + "\n";
		}
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Bounties])
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceToSpawnBountyFlat", playerData.instance.stats.ChanceToSpawnBounty.Total.RealValue.ToString("0")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("BountiesEffectAdditive", ((playerData.instance.stats.BountiesEffect.Total.RealValue - 1f) * 100f).ToString("0")) + "\n";
		}
		if (playerData.instance.stats.Debuff_DamageTaken_CanBeFoundInRun.Total.RealValue > 50)
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("ChanceToSpawnDebuffFlat", playerData.instance.stats.ChanceToSpawnDebuff.Total.RealValue.ToString("0.00")) + "\n";
		}
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Skills])
		{
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("SkillsCooldownAdditive", playerData.instance.stats.SkillsCooldown.AdditiveIncreases.RealValue.ToString("0")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("SkillsChanceForAnotherTriggerFlat", playerData.instance.stats.SkillsChanceForAnotherTrigger.Total.RealValue.ToString("0")) + "\n";
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("SkillCurrencyDropFlat", playerData.instance.stats.SkillCurrencyDrop.Total.RealValue.ToString("0")) + "\n";
		}
		if (playerData.instance.SkillIsUnlocked["LightningChain"])
		{
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("LightningChain_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("LC_ChainCountFlat", playerData.instance.stats.LC_ChainCount.Total.RealValue.ToString("0")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("LightningChain_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("LC_ChanceToForkAfterEachChainFlat", playerData.instance.stats.LC_ChanceToForkAfterEachChain.Total.RealValue.ToString("0.0")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("LightningChain_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("LC_DamageIncreasePerChainFlat", playerData.instance.stats.LC_DamageIncreasePerChain.Total.RealValue.ToString("0.00")) + "\n";
		}
		if (playerData.instance.SkillIsUnlocked["RainOfArrows"])
		{
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("RainOfArrows_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("RoA_CallCountFlat", playerData.instance.stats.RoA_CallCount.Total.RealValue.ToString("0")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("RainOfArrows_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("RoA_DamageIncreaseForEachCallCountFlat", playerData.instance.stats.RoA_DamageIncreaseForEachCallCount.Total.RealValue.ToString("0.00")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("RainOfArrows_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("RoA_ChanceForTwinFallFlat", playerData.instance.stats.RoA_ChanceForTwinFall.Total.RealValue.ToString("0.0")) + "\n";
		}
		if (playerData.instance.SkillIsUnlocked["KnightSlash"])
		{
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("KnightSlash_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("Knight_CallCountFlat", playerData.instance.stats.Knight_CallCount.Total.RealValue.ToString("0")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("KnightSlash_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("Knight_DamageIncreaseForEachCallCountFlat", playerData.instance.stats.Knight_DamageIncreaseForEachCallCount.Total.RealValue.ToString("0.00")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("KnightSlash_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("Knight_RadiusOfEffectAdditive", playerData.instance.stats.Knight_RadiusOfEffect.AdditiveIncreases.RealValue.ToString("0.00")) + "\n";
		}
		if (playerData.instance.SkillIsUnlocked["VampireExplosion"])
		{
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("VampireExplosion_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("Vampire_CallCountFlat", playerData.instance.stats.Vampire_CallCount.Total.RealValue.ToString("0")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("VampireExplosion_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("Vampire_DamageIncreaseForEachCallCountFlat", playerData.instance.stats.Vampire_DamageIncreaseForEachCallCount.Total.RealValue.ToString("0.00")) + "\n";
			text = text + "<b>" + LocalizerManager.GetTranslatedValue("VampireExplosion_Name") + "</b>: " + LocalizerManager.GetTranslatedThenReplaceValues("Vampire_RadiusOfEffectAdditive", playerData.instance.stats.Vampire_RadiusOfEffect.AdditiveIncreases.RealValue.ToString("0.00")) + "\n";
		}
		if ((double)playerData.instance.stats.DamageMultiplier_PerSecondInTimer.Total.RealValue > 0.01)
		{
			double num2 = playerData.instance.stats.DamageMultiplier_PerSecondInTimer.Total.RealValue * playerData.instance.stats.Timer.Total.RealValue;
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("DamageMultiplier_PerSecondInTimerFlat", playerData.instance.stats.DamageMultiplier_PerSecondInTimer.Total.RealValue.ToString("0")) + " (" + LocalizerManager.GetTranslatedThenReplaceValues("Total_Stats_Text", num2.ToString("0") + "%") + ")\n";
		}
		if ((double)playerData.instance.stats.DamageMultiplier_PerMonster.Total.RealValue > 0.01)
		{
			double num3 = playerData.instance.stats.DamageMultiplier_PerMonster.Total.RealValue * (float)playerData.instance.stats.NumberOfMonsters.Total.RealValue;
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("DamageMultiplier_PerMonsterFlat", playerData.instance.stats.DamageMultiplier_PerMonster.Total.RealValue.ToString("0")) + " (" + LocalizerManager.GetTranslatedThenReplaceValues("Total_Stats_Text", num3.ToString("0") + "%") + ")\n";
		}
		if ((double)playerData.instance.stats.DamageMultiplier_PerStatsInAllItemsEquipped.Total.RealValue > 0.01)
		{
			double num4 = playerData.instance.stats.DamageMultiplier_PerStatsInAllItemsEquipped.Total.RealValue * (float)playerData.instance.TotalStatsInItemsEquipped;
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("DamageMultiplier_PerStatsInAllItemsEquippedFlat", playerData.instance.stats.DamageMultiplier_PerStatsInAllItemsEquipped.Total.RealValue.ToString("0")) + " (" + LocalizerManager.GetTranslatedThenReplaceValues("Total_Stats_Text", num4.ToString("0") + "%") + ")\n";
		}
		if ((double)playerData.instance.stats.DamageMultiplier_PerLevelOfGems.Total.RealValue > 0.01)
		{
			double num5 = playerData.instance.stats.DamageMultiplier_PerLevelOfGems.Total.RealValue * (float)playerData.instance.TotalGemsLeveledUp;
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("DamageMultiplier_PerLevelOfGemsFlat", playerData.instance.stats.DamageMultiplier_PerLevelOfGems.Total.RealValue.ToString("0")) + " (" + LocalizerManager.GetTranslatedThenReplaceValues("Total_Stats_Text", num5.ToString("0") + "%") + ")\n";
		}
		if ((double)playerData.instance.stats.DamageMultiplier_PerTotalShinyFound.Total.RealValue > 0.01)
		{
			double num6 = playerData.instance.stats.DamageMultiplier_PerTotalShinyFound.Total.RealValue * (float)playerData.instance.TotalShinyFound;
			text = text + LocalizerManager.GetTranslatedThenReplaceValues("DamageMultiplier_PerTotalShinyFoundFlat", playerData.instance.stats.DamageMultiplier_PerTotalShinyFound.Total.RealValue.ToString("0.0")) + " (" + LocalizerManager.GetTranslatedThenReplaceValues("Total_Stats_Text", num6.ToString("0") + "%") + ")\n";
		}
		StatsText.text = text;
		AdjustTextHeight();
		UIManager.instance.CurrenciesTextManager();
	}

	public void AdjustTextHeight()
	{
		StatsText.ForceMeshUpdate();
		_ = StatsText.rectTransform;
		Vector2 preferredValues = StatsText.GetPreferredValues();
		ContentParent.sizeDelta = new Vector2(ContentParent.sizeDelta.x, preferredValues.y + 50f);
	}
}
public class TooltipController : MonoBehaviour
{
	public TextMeshProUGUI DescriptionText;

	public TextMeshProUGUI CostText;

	public TextMeshProUGUI LevelText;

	public Image TooltipBackground;

	public Color PurchaseableColor;

	public Color UnpurchaseableColor;

	public Color NormalDescriptionColor;

	public Color RequiredDescriptionColor;

	private RectTransform backgroundRectTransform;

	private float fixedHeight;

	private float widthOfTooltip;

	private float heightOfTooltip;

	private bool isInitialized;

	public TreeState treeState;

	[Header("Floating Animation")]
	public float floatingHeight = 15f;

	public float floatingSpeed = 2.5f;

	public float tiltAngle = 4f;

	public float tiltSpeed = 1.8f;

	[Header("Juicy Effects")]
	public float scaleAmplitude = 0.05f;

	public float scaleSpeed = 3f;

	[Header("Entrance/Exit Animation")]
	public float entranceDuration = 0.2f;

	public float exitDuration = 0.15f;

	[Header("Smart Positioning Thresholds")]
	public float topThresholdY = 450f;

	public float bottomThresholdY = -5000f;

	public float rightThresholdX = 500f;

	public float leftThresholdX = -950f;

	public float edgePadding = 20f;

	private Vector3 originalPosition;

	private Vector3 originalRotation;

	private Vector3 originalScale;

	private Tween floatingTween;

	private Tween tiltTween;

	private Tween scaleTween;

	private Tween entranceTween;

	private Tween exitTween;

	private bool isFloating;

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void init()
	{
		backgroundRectTransform = TooltipBackground.rectTransform;
		widthOfTooltip = backgroundRectTransform.sizeDelta.x;
		heightOfTooltip = backgroundRectTransform.sizeDelta.y;
		_ = CostText.rectTransform;
		if (originalScale == Vector3.zero)
		{
			originalScale = base.transform.localScale;
		}
		isInitialized = true;
	}

	public void ShowTooltip(TreeNodeInfo node, Vector2 PositionOfCenterUI, float additionalHeight)
	{
		if (!isInitialized)
		{
			init();
		}
		int nodeLevel = treeState.GetNodeLevel(node);
		base.gameObject.SetActive(value: true);
		LevelText.text = nodeLevel + " / " + node.NodeMaxLevel;
		DescriptionText.text = "";
		if (!playerData.instance.UnlockedSystems[node.RequiredUnlockableSystem])
		{
			DescriptionText.color = RequiredDescriptionColor;
			DescriptionText.text = LocalizerManager.GetTranslatedValue("Requirement_Text") + ": " + LocalizerManager.GetTranslatedValue(node.RequiredUnlockableSystem.ToString() + "_AzrarDesc");
		}
		else if (playerData.instance.MonstersLevel < node.RequiredMonsterLevel)
		{
			DescriptionText.color = RequiredDescriptionColor;
			DescriptionText.text = LocalizerManager.GetTranslatedValue("Requirement_Text") + ": " + LocalizerManager.GetTranslatedThenReplaceValues("MonstersLevel_Text", node.RequiredMonsterLevel.ToString());
		}
		else
		{
			for (int i = 0; i < node.NodeValueEquation.Count; i++)
			{
				double num = ExpressionEvaluator.Evaluate(node.NodeValueEquation[i], (nodeLevel <= 0) ? 1 : nodeLevel);
				if (node.GetNodeStat()[i].functionName == "MonstersSpawnRateFlat")
				{
					DescriptionText.text += node.GetNodeStat()[i].GetValueDescText_SingleOrMultipleValues(new List<double> { 1.0 / num }, isColoredTag: false);
				}
				else
				{
					DescriptionText.text += node.GetNodeStat()[i].GetValueDescText_SingleOrMultipleValues(new List<double> { num }, isColoredTag: false);
				}
				if (i < node.NodeValueEquation.Count - 1)
				{
					DescriptionText.text += "\n";
				}
			}
			if (node.GetNodeStat()[0].VariableName == "SkillCurrencyDrop" && playerData.instance.stats.SkillCurrencyDrop.Total.RealValue < 2 && playerData.instance.PlayerLevel > 0)
			{
				TextMeshProUGUI descriptionText = DescriptionText;
				descriptionText.text = descriptionText.text + "\n(" + LocalizerManager.GetTranslatedValue("AdditionalSkillCurrencyDrop_Text") + ": <sprite name=LevelPoints>" + playerData.instance.PlayerLevel + ")";
			}
			DescriptionText.color = NormalDescriptionColor;
		}
		if (nodeLevel < node.NodeMaxLevel)
		{
			List<double> list = new List<double>();
			for (int j = 0; j < node.NodeCostEquation.Count; j++)
			{
				double item = ExpressionEvaluator.Evaluate(node.NodeCostEquation[j], nodeLevel + 1);
				list.Add(item);
			}
			CostText.text = "";
			List<bool> item2 = PlayerManager.instance.IsCanSpendCurrencies(list, node.NodeCostCurrencies, isSpendAlso: false).Item2;
			for (int k = 0; k < list.Count; k++)
			{
				CostText.text += ("<sprite name=" + node.NodeCostCurrencies[k].ToString() + ">" + list[k].ToReadable()).ToColored(item2[k] ? PurchaseableColor : UnpurchaseableColor);
				if (k < list.Count - 1)
				{
					CostText.text += "   ";
				}
			}
		}
		else
		{
			CostText.text = LocalizerManager.GetTranslatedValue("Max_Text");
			CostText.color = PurchaseableColor;
		}
		heightOfTooltip = backgroundRectTransform.sizeDelta.y;
		Vector2 vector = CalculateSmartTooltipPosition(PositionOfCenterUI, additionalHeight);
		base.transform.position = vector;
		originalPosition = base.transform.position;
		originalRotation = Vector3.zero;
		base.transform.eulerAngles = originalRotation;
		if (originalScale.magnitude < 0.1f)
		{
			originalScale = base.transform.localScale;
		}
		StartEntranceAnimation();
	}

	private Vector2 CalculateSmartTooltipPosition(Vector2 nodePosition, float additionalHeight)
	{
		float num = widthOfTooltip / 2f;
		float num2 = heightOfTooltip / 2f;
		Vector2 result = nodePosition + new Vector2(0f, (additionalHeight + heightOfTooltip) / 2f);
		float num3 = result.y + num2;
		float num4 = result.y - num2;
		if (num3 > topThresholdY)
		{
			result.y = nodePosition.y - (additionalHeight + heightOfTooltip) / 2f;
		}
		else if (num4 < bottomThresholdY)
		{
			result.y = bottomThresholdY + num2;
		}
		float num5 = result.x + num;
		float num6 = result.x - num;
		if (num5 > rightThresholdX)
		{
			result.x = rightThresholdX - num;
		}
		else if (num6 < leftThresholdX)
		{
			result.x = leftThresholdX + num;
		}
		return result;
	}

	public void HideTooltip()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartExitAnimation();
		}
	}

	private void StartFloatingAnimation()
	{
		StopFloatingTweens();
		isFloating = true;
		StartDOTweenFloatingAnimation();
	}

	private void StopAllAnimations()
	{
		StopFloatingTweens();
		if (entranceTween != null)
		{
			entranceTween.Kill();
			entranceTween = null;
		}
		if (exitTween != null)
		{
			exitTween.Kill();
			exitTween = null;
		}
		isFloating = false;
		if (originalPosition.magnitude > 0.1f)
		{
			base.transform.position = originalPosition;
			base.transform.eulerAngles = originalRotation;
			base.transform.localScale = originalScale;
		}
	}

	private void StopFloatingTweens()
	{
		if (floatingTween != null)
		{
			floatingTween.Kill();
			floatingTween = null;
		}
		if (tiltTween != null)
		{
			tiltTween.Kill();
			tiltTween = null;
		}
		if (scaleTween != null)
		{
			scaleTween.Kill();
			scaleTween = null;
		}
	}

	private void StartEntranceAnimation()
	{
		StopAllAnimations();
		if (originalScale.magnitude < 0.1f)
		{
			originalScale = Vector3.one;
		}
		base.transform.localScale = Vector3.zero;
		entranceTween = base.transform.DOScale(originalScale, entranceDuration).SetEase(Ease.OutBack, 1.2f).OnComplete(delegate
		{
			entranceTween = null;
			StartFloatingAnimation();
		});
	}

	private void StartExitAnimation()
	{
		StopFloatingTweens();
		isFloating = false;
		if (exitTween != null)
		{
			exitTween.Kill();
			exitTween = null;
		}
		exitTween = base.transform.DOScale(Vector3.zero, exitDuration).SetEase(Ease.InBack, 1.2f).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
			base.transform.localScale = originalScale;
			exitTween = null;
		});
	}

	private void StartDOTweenFloatingAnimation()
	{
		if (isFloating)
		{
			base.transform.position = originalPosition;
			base.transform.eulerAngles = originalRotation;
			base.transform.localScale = originalScale;
			floatingTween = base.transform.DOMoveY(originalPosition.y + floatingHeight, 1f / floatingSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
				.From(originalPosition.y - floatingHeight);
			Vector3 eulerAngles = new Vector3(0f, 0f, 0f - tiltAngle);
			Vector3 endValue = new Vector3(0f, 0f, tiltAngle);
			base.transform.eulerAngles = eulerAngles;
			tiltTween = base.transform.DORotate(endValue, 1f / tiltSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			Vector3 endValue2 = originalScale * (1f + scaleAmplitude);
			scaleTween = base.transform.DOScale(endValue2, 1f / scaleSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}
	}
}
public class TutorialManager : SerializedMonoBehaviour
{
	public static TutorialManager instance;

	public Transform TutorialPanel;

	public Transform ShowHideTutorialButton;

	public GameObject ButtonsParent;

	public Dictionary<UnlockableSystems, Button> MenuButtons;

	private Dictionary<UnlockableSystems, Image> MenuImages;

	private Dictionary<UnlockableSystems, GameObject> LockIconGameObjects;

	private Dictionary<UnlockableSystems, GameObject> NormalIconGameObjects;

	public Color ButtonNormalColor;

	public Color ButtonHighlightedColor;

	public Color DisabledButtonColor;

	public Vector2 ShowPosition;

	public TextMeshProUGUI TitleText;

	public TextMeshProUGUI BodyText;

	private int CurrentClickedSystem;

	private bool isTutorialShown;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Debug.Log("Destroyed Instance");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		CurrentClickedSystem = -1;
		MenuImages = new Dictionary<UnlockableSystems, Image>();
		LockIconGameObjects = new Dictionary<UnlockableSystems, GameObject>();
		NormalIconGameObjects = new Dictionary<UnlockableSystems, GameObject>();
		foreach (KeyValuePair<UnlockableSystems, Button> menuButton in MenuButtons)
		{
			MenuImages.Add(menuButton.Key, menuButton.Value.GetComponent<Image>());
			LockIconGameObjects.Add(menuButton.Key, menuButton.Value.transform.Find("LockIcon").gameObject);
			NormalIconGameObjects.Add(menuButton.Key, menuButton.Value.transform.Find("MainIcon").gameObject);
		}
		CheckButtons();
		ShowHideTutorialButton.gameObject.SetActive(value: false);
		if (playerData.instance.UnlockedSystems[UnlockableSystems.Tree])
		{
			ShowHideTutorialButton.gameObject.SetActive(value: true);
			ClickedOnButton(0);
		}
	}

	public void CheckButtons()
	{
		bool flag = false;
		foreach (KeyValuePair<UnlockableSystems, Button> menuButton in MenuButtons)
		{
			menuButton.Value.interactable = playerData.instance.UnlockedSystems[menuButton.Key];
			MenuImages[menuButton.Key].color = (playerData.instance.UnlockedSystems[menuButton.Key] ? ButtonNormalColor : DisabledButtonColor);
			LockIconGameObjects[menuButton.Key].SetActive(!playerData.instance.UnlockedSystems[menuButton.Key]);
			NormalIconGameObjects[menuButton.Key].SetActive(playerData.instance.UnlockedSystems[menuButton.Key]);
			if (playerData.instance.UnlockedSystems[menuButton.Key])
			{
				flag = true;
			}
		}
		ButtonsParent.SetActive(flag);
		if (flag)
		{
			ShowHideTutorialButton.gameObject.SetActive(value: true);
		}
		ClickedOnButton((CurrentClickedSystem != -1) ? CurrentClickedSystem : 0);
	}

	public void ClickedOnButton(int system)
	{
		UnlockableSystems key = (UnlockableSystems)system;
		foreach (KeyValuePair<UnlockableSystems, Image> menuImage in MenuImages)
		{
			menuImage.Value.color = (playerData.instance.UnlockedSystems[menuImage.Key] ? ButtonNormalColor : DisabledButtonColor);
		}
		MenuImages[key].color = ButtonHighlightedColor;
		CurrentClickedSystem = system;
		TitleText.text = LocalizerManager.GetTranslatedValue(key.ToString() + "_Title_Tutorial");
		BodyText.text = LocalizerManager.GetTranslatedValue(key.ToString() + "_Body_Tutorial");
	}

	public void ClickedOnShowHideTutorial()
	{
		if (!TutorialPanel.gameObject.activeInHierarchy)
		{
			TutorialPanel.gameObject.SetActive(value: true);
		}
		isTutorialShown = !isTutorialShown;
		FunctionsNeeded.AnimateUIElement(isTutorialShown, TutorialPanel, ShowHideTutorialButton.position, ShowPosition);
	}

	public void ForceShowHideTutorial(bool isShow)
	{
		if (!TutorialPanel.gameObject.activeInHierarchy)
		{
			TutorialPanel.gameObject.SetActive(value: true);
		}
		FunctionsNeeded.AnimateUIElement(isShow, TutorialPanel, ShowHideTutorialButton.position, ShowPosition);
		isTutorialShown = isShow;
	}

	public void DelayedShowTutorial()
	{
		Invoke("ShowTutorial", 0.5f);
	}

	private void ShowTutorial()
	{
		if (!isTutorialShown && !RunManager.instance.isRunStarted)
		{
			ForceShowHideTutorial(isShow: true);
		}
	}
}
public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public TextMeshProUGUI GoldText;

	public TextMeshProUGUI TestAllCurrenciesText;

	public TextMeshProUGUI TestTimePlayedText;

	public TextMeshProUGUI EndOfRun_TotalCurrencies;

	public TextMeshProUGUI DebugStatsText_Run;

	public TextMeshProUGUI DebugStatsText_Hand;

	public GameObject RunEndPanel;

	public TextMeshProUGUI RunEnd_CurrenciesGained;

	public GameObject NonRunPanel;

	public GameObject RunPanel;

	public Image Blacklayout;

	public GameObject KillMonstersGO;

	public TextMeshProUGUI KillMonstersText;

	public TextMeshProUGUI CurrentLevelText;

	public GameObject RightButtonGO;

	public GameObject LeftButtonGO;

	[HideInInspector]
	public float InOutDuration = 0.25f;

	[HideInInspector]
	public Ease InOutEase = Ease.InOutCubic;

	[HideInInspector]
	public float XHidePosition = 2000f;

	private float BlacklayoutAlpha;

	private Dictionary<Currencies, bool> WasShownBefore = new Dictionary<Currencies, bool>();

	private bool isFilledTheWellThisRun;

	public Color KillText_NormalColor;

	public Color KillText_FilledWellColor;

	private float deltaTime;

	public void AwakeMe()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		foreach (Currencies value in Enum.GetValues(typeof(Currencies)))
		{
			WasShownBefore[value] = false;
			Dictionary<Currencies, Action> onCurrencyChange = PlayerManager.instance.OnCurrencyChange;
			Currencies key = value;
			onCurrencyChange[key] = (Action)Delegate.Combine(onCurrencyChange[key], new Action(CurrenciesTextManager));
		}
		playerData.instance.stats.OnStatsUpgrade += UpdateStatsTexts;
		UpdateStatsTexts();
		CurrenciesTextManager();
		BlacklayoutAlpha = Blacklayout.color.a;
		ManagePlayMonsterLevelTextAndButtons();
	}

	public void LoadFunction()
	{
		CurrenciesTextManager();
	}

	public void ManagePlayMonsterLevelTextAndButtons()
	{
		CurrentLevelText.text = LocalizerManager.GetTranslatedValue("Stage_Text") + " " + playerData.instance.MonstersLevel;
	}

	public void ManageKillText(bool isStartRun)
	{
		if (!RunManager.instance.isRunStarted || isFilledTheWellThisRun)
		{
			return;
		}
		if (RunManager.instance.IsBossRun && !playerData.instance.isFinishedTheGame)
		{
			KillMonstersGO.SetActive(value: true);
			int num = DatabaseManager.NumberOfBossesRequiredToFinishBossRun - playerData.instance.TotalMonstersKilled_CurrentRun;
			if (num == 0)
			{
				FXManager.instance.PlayGeneralSound(GeneralSounds.card_activation);
				KillMonstersText.GetComponent<Breather>().Pop();
			}
			if (num <= 0)
			{
				KillMonstersText.text = LocalizerManager.GetTranslatedValue("AllBossesAreKilled_Text");
			}
			else
			{
				KillMonstersText.text = LocalizerManager.GetTranslatedThenReplaceValues("KillBossesCount_Text", num.ToString());
			}
			return;
		}
		if (playerData.instance.MonstersLevel >= DatabaseManager.MaxMonstersLevelInGame)
		{
			if (playerData.instance.WellFillCount < DatabaseManager.MaxWellFillCount)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("#VALUE1#", DatabaseManager.MaxWellFillCount.ToString().ToString());
				dictionary.Add("#VALUE2#", (DatabaseManager.MaxWellFillCount - playerData.instance.WellFillCount).ToString());
				KillMonstersText.text = LocalizerManager.GetTranslatedThenReplaceValues("FillTheWellXTimesToUnlock_Text", dictionary);
			}
			else
			{
				KillMonstersText.text = LocalizerManager.GetTranslatedThenReplaceValues("KilledMonsters_Text", playerData.instance.TotalMonstersKilled_CurrentRun.ToString());
			}
			return;
		}
		int num2 = DatabaseManager.NumberOfMonstersToUnlock[playerData.instance.MonstersLevel + 1] - playerData.instance.TotalMonstersKilled_CurrentRun;
		if (num2 == 0)
		{
			FXManager.instance.PlayGeneralSound(GeneralSounds.card_activation);
			KillMonstersText.GetComponent<Breather>().Pop();
		}
		if (num2 <= 0)
		{
			KillMonstersText.text = LocalizerManager.GetTranslatedThenReplaceValues("LevelUnlocked_Text", (playerData.instance.MonstersLevel + 1).ToString());
			return;
		}
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2.Add("#VALUE1#", num2.ToString());
		dictionary2.Add("#VALUE2#", (playerData.instance.MonstersLevel + 1).ToString());
		KillMonstersText.text = LocalizerManager.GetTranslatedThenReplaceValues("KillCount_Text", dictionary2);
	}

	public void PopText_WellManager()
	{
		FXManager.instance.PlayGeneralSound(GeneralSounds.card_activation);
		KillMonstersText.GetComponent<Breather>().Pop();
		KillMonstersText.text = LocalizerManager.GetTranslatedValue("FillTheWellXTimesToUnlocked_Text");
		isFilledTheWellThisRun = true;
	}

	public void CurrenciesTextManager()
	{
		string text = "";
		if (WasShownBefore[Currencies.WellCurrency] || (playerData.instance.WellCurrency > 0 && !WasShownBefore[Currencies.WellCurrency]))
		{
			text = text + "   <sprite name=WellCurrency>" + playerData.instance.WellCurrency;
			WasShownBefore[Currencies.WellCurrency] = true;
		}
		if (WasShownBefore[Currencies.GemCurrency] || (playerData.instance.GemCurrency > 0 && !WasShownBefore[Currencies.GemCurrency]))
		{
			text = text + "   <sprite name=GemCurrency>" + playerData.instance.GemCurrency;
			WasShownBefore[Currencies.GemCurrency] = true;
		}
		if (WasShownBefore[Currencies.CharacterCurrency] || (playerData.instance.CharacterCurrency > 0 && !WasShownBefore[Currencies.CharacterCurrency]))
		{
			text = text + "   <sprite name=CharacterCurrency>" + playerData.instance.CharacterCurrency;
			WasShownBefore[Currencies.CharacterCurrency] = true;
		}
		if (WasShownBefore[Currencies.LevelPoints] || (playerData.instance.LevelPoints > 0 && !WasShownBefore[Currencies.LevelPoints]))
		{
			text = text + "   <sprite name=LevelPoints>" + playerData.instance.LevelPoints;
			WasShownBefore[Currencies.LevelPoints] = true;
		}
		if (WasShownBefore[Currencies.ClearCurrency] || (playerData.instance.ClearCurrency > 0 && !WasShownBefore[Currencies.ClearCurrency]))
		{
			text = text + "   <sprite name=ClearCurrency>" + playerData.instance.ClearCurrency;
			WasShownBefore[Currencies.ClearCurrency] = true;
		}
		text = text + "   <sprite name=Gold>" + playerData.instance.PlayerGold.ToReadable();
		text = text.TrimStart();
		EndOfRun_TotalCurrencies.text = LocalizerManager.GetTranslatedValue("Total_Text") + ": " + text;
		double num = 1.0;
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerTotalShinyFound.Total.RealValue * (float)playerData.instance.TotalShinyFound / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerStatsInAllItemsEquipped.Total.RealValue * (float)playerData.instance.TotalStatsInItemsEquipped / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerBountyFound.Total.RealValue * (float)playerData.instance.TotalBountiesFound_CurrentRun / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerLevelOfGems.Total.RealValue * (float)playerData.instance.TotalGemsLeveledUp / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerSecondInTimer.Total.RealValue * playerData.instance.stats.Timer.Total.RealValue / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerAreaMarkApplied.Total.RealValue * (float)playerData.instance.TotalAreaMarksApplied_CurrentRun / 100f);
		num *= (double)(1f + playerData.instance.stats.DamageMultiplier_PerMonster.Total.RealValue * (float)playerData.instance.stats.NumberOfMonsters.Total.RealValue / 100f);
		double number = playerData.instance.stats.Damage.Total.RealValue * num;
		text = text + "   <sprite name=Damage>" + number.ToReadable();
		GoldText.text = text;
	}

	private void TestAllCurrenciesTextManager()
	{
		string text = "";
		foreach (Currencies value in Enum.GetValues(typeof(Currencies)))
		{
			text = text + "   <sprite name=" + value.ToString() + ">" + playerData.instance.TotalCurrenciesGained_FullGame[value].ToReadable();
		}
		TestAllCurrenciesText.text = text + " Killed: " + playerData.instance.TotalMonstersKilled_FullGame;
	}

	public void ClickedOnStartRun()
	{
		FXManager.instance.PlayGeneralSound(GeneralSounds.menu_whoosh);
		ShowHideTransformsPanel(NonRunPanel.transform, isShow: false);
		ShowHideTransformsPanel(RunEndPanel.transform, isShow: false);
		ShowHideTransformsPanel(RunPanel.transform, isShow: true);
		ShowHideBlacklayout(isShow: false, isForce: false);
		RunManager.instance.StartRun();
		if (playerData.instance.MonstersLevel >= DatabaseManager.MaxMonstersLevelInGame && playerData.instance.WellFillCount < DatabaseManager.MaxWellFillCount)
		{
			KillMonstersText.color = KillText_FilledWellColor;
		}
		else
		{
			KillMonstersText.color = KillText_NormalColor;
		}
	}

	public void ClickedOnPlayAgain()
	{
		ClickedOnStartRun();
	}

	public void ClickedOnGoToMainMenu()
	{
		FXManager.instance.PlayGeneralSound(GeneralSounds.menu_whoosh);
		RunManager.instance.IsBossRun = false;
		ShowHideTransformsPanel(RunEndPanel.transform, isShow: false);
		ShowHideTransformsPanel(NonRunPanel.transform, isShow: true);
		ShowHideTransformsPanel(RunPanel.transform, isShow: false);
		ShowHideBlacklayout(isShow: false, isForce: false);
		EnemiesManager.instance.DeSpawnAllEnemies();
		StartCoroutine(AzrarManager.instance.ClickedOnEndRun());
	}

	public void ShowEndRunPanel(string currenciesGained)
	{
		isFilledTheWellThisRun = false;
		FXManager.instance.PlayGeneralSound(GeneralSounds.menu_whoosh);
		FXManager.instance.PlayGeneralSound(GeneralSounds.gold_gain_one);
		ShowHideBlacklayout(isShow: true, isForce: false);
		ShowHideTransformsPanel(RunEndPanel.transform, isShow: true);
		ShowHideTransformsPanel(NonRunPanel.transform, isShow: false);
		RunEnd_CurrenciesGained.text = currenciesGained;
	}

	private void ShowHideTransformsPanel(Transform TheTransform, bool isShow)
	{
		CanvasGroup canvasGroup = TheTransform.GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = TheTransform.gameObject.AddComponent<CanvasGroup>();
		}
		if (isShow)
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			TheTransform.DOLocalMoveX(0f, InOutDuration).SetEase(InOutEase);
			canvasGroup.DOFade(1f, InOutDuration);
		}
		else
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			TheTransform.DOLocalMoveX(XHidePosition, InOutDuration).SetEase(InOutEase);
			canvasGroup.DOFade(0f, InOutDuration);
		}
	}

	public void ShowHideBlacklayout(bool isShow, bool isForce)
	{
		if (isForce)
		{
			Blacklayout.gameObject.SetActive(isShow);
		}
		else if (isShow)
		{
			Blacklayout.gameObject.SetActive(value: true);
			Color color = Blacklayout.color;
			color.a = 0f;
			Blacklayout.color = color;
			Blacklayout.DOFade(BlacklayoutAlpha, InOutDuration).SetEase(InOutEase);
		}
		else if (Blacklayout.gameObject.activeInHierarchy)
		{
			Blacklayout.DOFade(0f, InOutDuration).SetEase(InOutEase).OnComplete(delegate
			{
				Blacklayout.gameObject.SetActive(value: false);
			});
		}
	}

	public void UpdateStatsTexts()
	{
	}

	private void Update()
	{
	}
}
public class ZoomContent : MonoBehaviour, IScrollHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IPointerClickHandler
{
	private RectTransform target;

	private Canvas canvas;

	[Header("Zoom Settings")]
	public float zoomSpeed = 0.01f;

	public float minZoom = 0.75f;

	public float maxZoom = 1.5f;

	[Header("Pan Settings")]
	public float panSpeed = 1f;

	public float maxOffsetX = 1000f;

	public float maxOffsetY = 1000f;

	private Vector2 lastMousePos;

	private Camera uiCamera;

	private Vector3 _initialScale;

	private Vector3 _initialPosition;

	private void Awake()
	{
		if (target == null)
		{
			target = GetComponent<RectTransform>();
		}
		if (canvas == null)
		{
			canvas = GetComponentInParent<Canvas>();
		}
		uiCamera = ((canvas != null && canvas.renderMode != 0) ? canvas.worldCamera : null);
		_initialScale = target.localScale;
		_initialPosition = target.localPosition;
	}

	public void ResetPanZoom()
	{
		target.localScale = _initialScale;
		target.localPosition = _initialPosition;
	}

	public void OnScroll(PointerEventData eventData)
	{
		float y = eventData.scrollDelta.y;
		if (!Mathf.Approximately(y, 0f) && RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, uiCamera, out var localPoint))
		{
			Vector3 vector = target.TransformPoint(localPoint);
			float num = 1f + y * zoomSpeed;
			Vector3 localScale = target.localScale * num;
			localScale.x = Mathf.Clamp(localScale.x, minZoom, maxZoom);
			localScale.y = Mathf.Clamp(localScale.y, minZoom, maxZoom);
			localScale.z = 1f;
			target.localScale = localScale;
			Vector3 vector2 = target.TransformPoint(localPoint);
			target.position += vector - vector2;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		lastMousePos = eventData.position;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 position = eventData.position;
		Vector2 vector = position - lastMousePos;
		float num = ((canvas != null) ? canvas.scaleFactor : 1f);
		Vector3 localPosition = target.localPosition + (Vector3)(vector / num) * panSpeed;
		localPosition.x = Mathf.Clamp(localPosition.x, 0f - maxOffsetX, maxOffsetX);
		localPosition.y = Mathf.Clamp(localPosition.y, 0f - maxOffsetY, maxOffsetY);
		target.localPosition = localPosition;
		lastMousePos = position;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Middle)
		{
			ResetPanZoom();
		}
	}
}
[CompilerGenerated]
[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("Unity.MonoScriptGenerator.MonoScriptInfoGenerator", null)]
internal class UnitySourceGeneratedAssemblyMonoScriptTypes_v1
{
	private struct MonoScriptData
	{
		public byte[] FilePathsData;

		public byte[] TypesData;

		public int TotalTypes;

		public int TotalFiles;

		public bool IsEditorOnly;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static MonoScriptData Get()
	{
		MonoScriptData result = default(MonoScriptData);
		result.FilePathsData = new byte[4955]
		{
			0, 0, 0, 1, 0, 0, 0, 42, 92, 65,
			115, 115, 101, 116, 115, 92, 77, 121, 76, 111,
			99, 97, 108, 105, 122, 101, 114, 92, 76, 111,
			99, 97, 108, 105, 122, 101, 100, 84, 101, 120,
			116, 79, 98, 106, 101, 99, 116, 46, 99, 115,
			0, 0, 0, 5, 0, 0, 0, 39, 92, 65,
			115, 115, 101, 116, 115, 92, 77, 121, 76, 111,
			99, 97, 108, 105, 122, 101, 114, 92, 76, 111,
			99, 97, 108, 105, 122, 101, 114, 77, 97, 110,
			97, 103, 101, 114, 46, 99, 115, 0, 0, 0,
			1, 0, 0, 0, 46, 92, 65, 115, 115, 101,
			116, 115, 92, 83, 99, 114, 105, 112, 116, 115,
			92, 83, 116, 101, 97, 109, 119, 111, 114, 107,
			115, 46, 78, 69, 84, 92, 83, 116, 101, 97,
			109, 77, 97, 110, 97, 103, 101, 114, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 47, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 65, 110,
			105, 109, 97, 116, 105, 111, 110, 115, 92, 116,
			101, 115, 116, 65, 114, 99, 104, 101, 114, 92,
			116, 101, 115, 116, 65, 114, 99, 104, 101, 114,
			65, 105, 109, 46, 99, 115, 0, 0, 0, 9,
			0, 0, 0, 35, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 65, 122, 114, 97, 114, 92, 65, 122, 114,
			97, 114, 73, 110, 102, 111, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 38, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 65, 122, 114, 97, 114, 92,
			65, 122, 114, 97, 114, 77, 97, 110, 97, 103,
			101, 114, 46, 99, 115, 0, 0, 0, 1, 0,
			0, 0, 37, 92, 65, 115, 115, 101, 116, 115,
			92, 95, 83, 99, 114, 105, 112, 116, 115, 92,
			65, 122, 114, 97, 114, 92, 65, 122, 114, 97,
			114, 83, 101, 108, 102, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 45, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 67, 104, 97, 114, 97,
			99, 116, 101, 114, 92, 67, 104, 97, 114, 97,
			99, 116, 101, 114, 83, 101, 108, 102, 101, 114,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			47, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 67, 104,
			97, 114, 97, 99, 116, 101, 114, 92, 67, 104,
			97, 114, 97, 99, 116, 101, 114, 83, 116, 97,
			116, 73, 110, 102, 111, 46, 99, 115, 0, 0,
			0, 1, 0, 0, 0, 48, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 67, 104, 97, 114, 97, 99, 116,
			101, 114, 92, 67, 104, 97, 114, 97, 99, 116,
			101, 114, 85, 73, 77, 97, 110, 97, 103, 101,
			114, 46, 99, 115, 0, 0, 0, 1, 0, 0,
			0, 40, 92, 65, 115, 115, 101, 116, 115, 92,
			95, 83, 99, 114, 105, 112, 116, 115, 92, 69,
			110, 101, 109, 121, 92, 69, 110, 101, 109, 105,
			101, 115, 77, 97, 110, 97, 103, 101, 114, 46,
			99, 115, 0, 0, 0, 1, 0, 0, 0, 35,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 69, 110, 101,
			109, 121, 92, 69, 110, 101, 109, 121, 73, 110,
			102, 111, 46, 99, 115, 0, 0, 0, 1, 0,
			0, 0, 37, 92, 65, 115, 115, 101, 116, 115,
			92, 95, 83, 99, 114, 105, 112, 116, 115, 92,
			69, 110, 101, 109, 121, 92, 69, 110, 101, 109,
			121, 83, 101, 108, 102, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 35, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 69, 110, 101, 109, 121,
			92, 83, 104, 105, 110, 121, 73, 110, 102, 111,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			38, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 69, 110,
			101, 109, 121, 92, 83, 104, 105, 110, 121, 77,
			97, 110, 97, 103, 101, 114, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 52, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 69, 115, 115, 101, 110, 116,
			105, 97, 108, 92, 67, 114, 111, 115, 115, 80,
			108, 97, 116, 102, 111, 114, 109, 70, 117, 110,
			99, 116, 105, 111, 110, 115, 46, 99, 115, 0,
			0, 0, 3, 0, 0, 0, 45, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 69, 115, 115, 101, 110, 116,
			105, 97, 108, 92, 68, 97, 116, 97, 98, 97,
			115, 101, 77, 97, 110, 97, 103, 101, 114, 46,
			99, 115, 0, 0, 0, 9, 0, 0, 0, 45,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 69, 115, 115,
			101, 110, 116, 105, 97, 108, 92, 70, 117, 110,
			99, 116, 105, 111, 110, 115, 78, 101, 101, 100,
			101, 100, 46, 99, 115, 0, 0, 0, 1, 0,
			0, 0, 54, 92, 65, 115, 115, 101, 116, 115,
			92, 95, 83, 99, 114, 105, 112, 116, 115, 92,
			69, 115, 115, 101, 110, 116, 105, 97, 108, 92,
			70, 117, 110, 99, 116, 105, 111, 110, 115, 79,
			102, 84, 104, 71, 97, 109, 101, 77, 97, 110,
			97, 103, 101, 114, 46, 99, 115, 0, 0, 0,
			1, 0, 0, 0, 46, 92, 65, 115, 115, 101,
			116, 115, 92, 95, 83, 99, 114, 105, 112, 116,
			115, 92, 69, 115, 115, 101, 110, 116, 105, 97,
			108, 92, 77, 97, 110, 97, 103, 101, 114, 79,
			102, 84, 104, 101, 71, 97, 109, 101, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 42, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 69, 115, 115, 101,
			110, 116, 105, 97, 108, 92, 79, 98, 106, 101,
			99, 116, 80, 111, 111, 108, 101, 114, 46, 99,
			115, 0, 0, 0, 28, 0, 0, 0, 40, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 69, 115, 115, 101,
			110, 116, 105, 97, 108, 92, 112, 108, 97, 121,
			101, 114, 68, 97, 116, 97, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 45, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 69, 115, 115, 101, 110, 116,
			105, 97, 108, 92, 83, 97, 118, 101, 76, 111,
			97, 100, 77, 97, 110, 97, 103, 101, 114, 46,
			99, 115, 0, 0, 0, 3, 0, 0, 0, 52,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 69, 115, 115,
			101, 110, 116, 105, 97, 108, 92, 83, 101, 114,
			105, 97, 108, 105, 122, 97, 98, 108, 101, 68,
			105, 99, 116, 105, 111, 110, 97, 114, 121, 46,
			99, 115, 0, 0, 0, 1, 0, 0, 0, 51,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 69, 115, 115,
			101, 110, 116, 105, 97, 108, 92, 83, 116, 97,
			116, 101, 79, 102, 84, 104, 101, 71, 97, 109,
			101, 77, 97, 110, 97, 103, 101, 114, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 41, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 71, 97, 109, 101,
			112, 108, 97, 121, 92, 65, 117, 116, 111, 109,
			97, 116, 111, 114, 66, 111, 116, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 42, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 71, 97, 109, 101, 112,
			108, 97, 121, 92, 70, 105, 110, 105, 115, 104,
			77, 97, 110, 97, 103, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 39, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 71, 97, 109, 101, 112,
			108, 97, 121, 92, 82, 117, 110, 77, 97, 110,
			97, 103, 101, 114, 46, 99, 115, 0, 0, 0,
			1, 0, 0, 0, 37, 92, 65, 115, 115, 101,
			116, 115, 92, 95, 83, 99, 114, 105, 112, 116,
			115, 92, 71, 97, 109, 101, 112, 108, 97, 121,
			92, 87, 101, 108, 108, 73, 110, 102, 111, 46,
			99, 115, 0, 0, 0, 1, 0, 0, 0, 40,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 71, 97, 109,
			101, 112, 108, 97, 121, 92, 87, 101, 108, 108,
			77, 97, 110, 97, 103, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 36, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 71, 101, 109, 115, 92,
			71, 101, 109, 115, 77, 97, 110, 97, 103, 101,
			114, 46, 99, 115, 0, 0, 0, 2, 0, 0,
			0, 36, 92, 65, 115, 115, 101, 116, 115, 92,
			95, 83, 99, 114, 105, 112, 116, 115, 92, 71,
			101, 109, 115, 92, 71, 101, 109, 83, 116, 97,
			116, 73, 110, 102, 111, 46, 99, 115, 0, 0,
			0, 2, 0, 0, 0, 42, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 73, 116, 101, 109, 115, 92, 73,
			110, 118, 101, 110, 116, 111, 114, 121, 77, 97,
			110, 97, 103, 101, 114, 46, 99, 115, 0, 0,
			0, 2, 0, 0, 0, 39, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 73, 116, 101, 109, 115, 92, 73,
			116, 101, 109, 83, 116, 97, 116, 115, 73, 110,
			102, 111, 46, 99, 115, 0, 0, 0, 1, 0,
			0, 0, 35, 92, 65, 115, 115, 101, 116, 115,
			92, 95, 83, 99, 114, 105, 112, 116, 115, 92,
			76, 101, 118, 101, 108, 92, 76, 101, 118, 101,
			108, 73, 110, 102, 111, 46, 99, 115, 0, 0,
			0, 1, 0, 0, 0, 45, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 79, 116, 104, 101, 114, 92, 65,
			99, 104, 105, 101, 118, 101, 109, 101, 110, 116,
			115, 77, 97, 110, 97, 103, 101, 114, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 48, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 79, 116, 104, 101,
			114, 92, 70, 108, 111, 97, 116, 105, 110, 103,
			78, 117, 109, 98, 101, 114, 115, 77, 97, 110,
			97, 103, 101, 114, 46, 99, 115, 0, 0, 0,
			1, 0, 0, 0, 35, 92, 65, 115, 115, 101,
			116, 115, 92, 95, 83, 99, 114, 105, 112, 116,
			115, 92, 79, 116, 104, 101, 114, 92, 70, 88,
			77, 97, 110, 97, 103, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 45, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 79, 116, 104, 101, 114,
			92, 71, 114, 111, 117, 110, 100, 67, 108, 105,
			99, 107, 97, 98, 108, 101, 73, 110, 102, 111,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			48, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 79, 116,
			104, 101, 114, 92, 71, 114, 111, 117, 110, 100,
			67, 108, 105, 99, 107, 97, 98, 108, 101, 77,
			97, 110, 97, 103, 101, 114, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 47, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 79, 116, 104, 101, 114, 92,
			71, 114, 111, 117, 110, 100, 67, 108, 105, 99,
			107, 97, 98, 108, 101, 83, 101, 108, 102, 101,
			114, 46, 99, 115, 0, 0, 0, 1, 0, 0,
			0, 44, 92, 65, 115, 115, 101, 116, 115, 92,
			95, 83, 99, 114, 105, 112, 116, 115, 92, 79,
			116, 104, 101, 114, 92, 71, 114, 111, 117, 110,
			100, 69, 102, 102, 101, 99, 116, 83, 101, 108,
			102, 101, 114, 46, 99, 115, 0, 0, 0, 2,
			0, 0, 0, 46, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 79, 116, 104, 101, 114, 92, 71, 114, 111,
			117, 110, 100, 69, 102, 102, 101, 99, 116, 115,
			77, 97, 110, 97, 103, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 40, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 79, 116, 104, 101, 114,
			92, 76, 111, 103, 103, 105, 110, 103, 77, 97,
			110, 97, 103, 101, 114, 46, 99, 115, 0, 0,
			0, 1, 0, 0, 0, 34, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 79, 116, 104, 101, 114, 92, 80,
			83, 83, 101, 108, 102, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 43, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 79, 116, 104, 101, 114,
			92, 82, 97, 110, 100, 111, 109, 65, 117, 100,
			105, 111, 83, 101, 108, 102, 101, 114, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 37, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 79, 116, 104, 101,
			114, 92, 83, 111, 117, 110, 100, 83, 101, 108,
			102, 101, 114, 46, 99, 115, 0, 0, 0, 1,
			0, 0, 0, 42, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 79, 116, 104, 101, 114, 92, 84, 101, 120,
			116, 83, 101, 108, 102, 70, 108, 111, 97, 116,
			105, 110, 103, 46, 99, 115, 0, 0, 0, 1,
			0, 0, 0, 49, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 79, 116, 104, 101, 114, 92, 84, 114, 97,
			105, 108, 65, 100, 100, 77, 117, 108, 116, 105,
			67, 111, 110, 116, 114, 111, 108, 108, 101, 114,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			40, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 80, 108,
			97, 121, 101, 114, 92, 68, 101, 98, 117, 102,
			102, 77, 97, 110, 97, 103, 101, 114, 46, 99,
			115, 0, 0, 0, 2, 0, 0, 0, 40, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 80, 108, 97, 121,
			101, 114, 92, 77, 111, 117, 115, 101, 65, 116,
			116, 97, 99, 107, 101, 114, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 40, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 80, 108, 97, 121, 101, 114,
			92, 80, 108, 97, 121, 101, 114, 77, 97, 110,
			97, 103, 101, 114, 46, 99, 115, 0, 0, 0,
			1, 0, 0, 0, 49, 92, 65, 115, 115, 101,
			116, 115, 92, 95, 83, 99, 114, 105, 112, 116,
			115, 92, 80, 114, 111, 106, 101, 99, 116, 105,
			108, 101, 92, 80, 114, 111, 106, 101, 99, 116,
			105, 108, 101, 66, 101, 104, 97, 118, 105, 111,
			114, 46, 99, 115, 0, 0, 0, 1, 0, 0,
			0, 45, 92, 65, 115, 115, 101, 116, 115, 92,
			95, 83, 99, 114, 105, 112, 116, 115, 92, 80,
			114, 111, 106, 101, 99, 116, 105, 108, 101, 92,
			80, 114, 111, 106, 101, 99, 116, 105, 108, 101,
			73, 110, 102, 111, 46, 99, 115, 0, 0, 0,
			1, 0, 0, 0, 46, 92, 65, 115, 115, 101,
			116, 115, 92, 95, 83, 99, 114, 105, 112, 116,
			115, 92, 80, 114, 111, 106, 101, 99, 116, 105,
			108, 101, 92, 80, 114, 111, 106, 101, 99, 116,
			105, 108, 101, 77, 111, 118, 101, 114, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 47, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 80, 114, 111, 106,
			101, 99, 116, 105, 108, 101, 92, 80, 114, 111,
			106, 101, 99, 116, 105, 108, 101, 83, 101, 108,
			102, 101, 114, 46, 99, 115, 0, 0, 0, 1,
			0, 0, 0, 49, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 80, 114, 111, 106, 101, 99, 116, 105, 108,
			101, 92, 80, 114, 111, 106, 101, 99, 116, 105,
			108, 101, 115, 77, 97, 110, 97, 103, 101, 114,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			47, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 80, 114,
			111, 106, 101, 99, 116, 105, 108, 101, 92, 83,
			107, 105, 108, 108, 66, 97, 114, 115, 77, 97,
			110, 97, 103, 101, 114, 46, 99, 115, 0, 0,
			0, 2, 0, 0, 0, 46, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 80, 114, 111, 106, 101, 99, 116,
			105, 108, 101, 92, 83, 107, 105, 108, 108, 68,
			101, 116, 97, 105, 108, 73, 110, 102, 111, 46,
			99, 115, 0, 0, 0, 1, 0, 0, 0, 40,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 80, 114, 111,
			106, 101, 99, 116, 105, 108, 101, 92, 83, 107,
			105, 108, 108, 73, 110, 102, 111, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 44, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 80, 114, 111, 106, 101,
			99, 116, 105, 108, 101, 92, 83, 107, 105, 108,
			108, 115, 77, 97, 110, 97, 103, 101, 114, 46,
			99, 115, 0, 0, 0, 1, 0, 0, 0, 46,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 80, 114, 111,
			106, 101, 99, 116, 105, 108, 101, 92, 83, 107,
			105, 108, 108, 115, 85, 73, 77, 97, 110, 97,
			103, 101, 114, 46, 99, 115, 0, 0, 0, 1,
			0, 0, 0, 40, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 83, 116, 97, 116, 115, 92, 69, 110, 101,
			109, 121, 83, 116, 97, 116, 115, 68, 97, 116,
			97, 46, 99, 115, 0, 0, 0, 1, 0, 0,
			0, 41, 92, 65, 115, 115, 101, 116, 115, 92,
			95, 83, 99, 114, 105, 112, 116, 115, 92, 83,
			116, 97, 116, 115, 92, 80, 108, 97, 121, 101,
			114, 83, 116, 97, 116, 115, 68, 97, 116, 97,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			34, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 83, 116,
			97, 116, 115, 92, 83, 116, 97, 116, 73, 110,
			102, 111, 46, 99, 115, 0, 0, 0, 9, 0,
			0, 0, 35, 92, 65, 115, 115, 101, 116, 115,
			92, 95, 83, 99, 114, 105, 112, 116, 115, 92,
			83, 116, 97, 116, 115, 92, 83, 116, 97, 116,
			115, 68, 97, 116, 97, 46, 99, 115, 0, 0,
			0, 1, 0, 0, 0, 50, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 83, 116, 97, 116, 115, 92, 83,
			116, 97, 116, 115, 73, 110, 116, 84, 111, 70,
			108, 111, 97, 116, 67, 111, 110, 118, 101, 114,
			116, 101, 114, 46, 99, 115, 0, 0, 0, 1,
			0, 0, 0, 54, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 84, 111, 119, 101, 114, 115, 92, 84, 111,
			119, 101, 114, 67, 105, 114, 99, 108, 101, 80,
			114, 111, 106, 101, 99, 116, 105, 108, 101, 83,
			101, 108, 102, 101, 114, 46, 99, 115, 0, 0,
			0, 1, 0, 0, 0, 36, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 84, 111, 119, 101, 114, 115, 92,
			84, 111, 119, 101, 114, 73, 110, 102, 111, 46,
			99, 115, 0, 0, 0, 1, 0, 0, 0, 38,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 84, 111, 119,
			101, 114, 115, 92, 84, 111, 119, 101, 114, 83,
			101, 108, 102, 101, 114, 46, 99, 115, 0, 0,
			0, 1, 0, 0, 0, 40, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 84, 111, 119, 101, 114, 115, 92,
			84, 111, 119, 101, 114, 115, 77, 97, 110, 97,
			103, 101, 114, 46, 99, 115, 0, 0, 0, 2,
			0, 0, 0, 44, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 84, 114, 101, 101, 92, 69, 120, 112, 114,
			101, 115, 115, 105, 111, 110, 69, 118, 97, 108,
			117, 97, 116, 111, 114, 46, 99, 115, 0, 0,
			0, 1, 0, 0, 0, 46, 92, 65, 115, 115,
			101, 116, 115, 92, 95, 83, 99, 114, 105, 112,
			116, 115, 92, 84, 114, 101, 101, 92, 84, 114,
			101, 101, 67, 114, 101, 97, 116, 111, 114, 46,
			67, 101, 110, 116, 101, 114, 105, 110, 103, 46,
			99, 115, 0, 0, 0, 2, 0, 0, 0, 36,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 84, 114, 101,
			101, 92, 84, 114, 101, 101, 67, 114, 101, 97,
			116, 111, 114, 46, 99, 115, 0, 0, 0, 1,
			0, 0, 0, 49, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 84, 114, 101, 101, 92, 84, 114, 101, 101,
			67, 114, 101, 97, 116, 111, 114, 46, 68, 111,
			109, 97, 105, 110, 82, 101, 108, 111, 97, 100,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			50, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 84, 114,
			101, 101, 92, 84, 114, 101, 101, 67, 114, 101,
			97, 116, 111, 114, 46, 69, 118, 101, 110, 116,
			72, 97, 110, 100, 108, 105, 110, 103, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 51, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 84, 114, 101, 101,
			92, 84, 114, 101, 101, 67, 114, 101, 97, 116,
			111, 114, 46, 76, 105, 110, 107, 77, 97, 110,
			97, 103, 101, 109, 101, 110, 116, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 51, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 84, 114, 101, 101, 92,
			84, 114, 101, 101, 67, 114, 101, 97, 116, 111,
			114, 46, 78, 111, 100, 101, 77, 97, 110, 97,
			103, 101, 109, 101, 110, 116, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 52, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 84, 114, 101, 101, 92, 84,
			114, 101, 101, 67, 114, 101, 97, 116, 111, 114,
			46, 83, 121, 110, 99, 104, 114, 111, 110, 105,
			122, 97, 116, 105, 111, 110, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 33, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 84, 114, 101, 101, 92, 84,
			114, 101, 101, 73, 110, 102, 111, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 33, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 84, 114, 101, 101, 92,
			84, 114, 101, 101, 76, 105, 110, 107, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 36, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 84, 114, 101, 101,
			92, 84, 114, 101, 101, 77, 97, 110, 97, 103,
			101, 114, 46, 99, 115, 0, 0, 0, 1, 0,
			0, 0, 33, 92, 65, 115, 115, 101, 116, 115,
			92, 95, 83, 99, 114, 105, 112, 116, 115, 92,
			84, 114, 101, 101, 92, 84, 114, 101, 101, 78,
			111, 100, 101, 46, 99, 115, 0, 0, 0, 2,
			0, 0, 0, 37, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 84, 114, 101, 101, 92, 84, 114, 101, 101,
			78, 111, 100, 101, 73, 110, 102, 111, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 34, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 84, 114, 101, 101,
			92, 84, 114, 101, 101, 83, 116, 97, 116, 101,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			39, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 84, 114,
			101, 101, 92, 85, 73, 76, 105, 110, 101, 82,
			101, 110, 100, 101, 114, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 32, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 85, 73, 92, 66, 97,
			114, 83, 101, 108, 102, 101, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 31, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 85, 73, 92, 66, 114,
			101, 97, 116, 104, 101, 114, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 48, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 85, 73, 92, 66, 117, 116,
			116, 111, 110, 80, 114, 101, 115, 115, 101, 100,
			77, 111, 118, 101, 84, 101, 120, 116, 68, 111,
			119, 110, 46, 99, 115, 0, 0, 0, 1, 0,
			0, 0, 37, 92, 65, 115, 115, 101, 116, 115,
			92, 95, 83, 99, 114, 105, 112, 116, 115, 92,
			85, 73, 92, 67, 117, 115, 116, 111, 109, 65,
			110, 105, 109, 97, 116, 111, 114, 46, 99, 115,
			0, 0, 0, 1, 0, 0, 0, 36, 92, 65,
			115, 115, 101, 116, 115, 92, 95, 83, 99, 114,
			105, 112, 116, 115, 92, 85, 73, 92, 72, 111,
			118, 101, 114, 65, 110, 105, 109, 97, 116, 111,
			114, 46, 99, 115, 0, 0, 0, 1, 0, 0,
			0, 39, 92, 65, 115, 115, 101, 116, 115, 92,
			95, 83, 99, 114, 105, 112, 116, 115, 92, 85,
			73, 92, 77, 97, 105, 110, 77, 101, 110, 117,
			115, 77, 97, 110, 97, 103, 101, 114, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 32, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 85, 73, 92, 79,
			110, 72, 111, 118, 101, 114, 85, 73, 46, 99,
			115, 0, 0, 0, 1, 0, 0, 0, 37, 92,
			65, 115, 115, 101, 116, 115, 92, 95, 83, 99,
			114, 105, 112, 116, 115, 92, 85, 73, 92, 79,
			110, 72, 111, 118, 101, 114, 85, 73, 83, 111,
			117, 110, 100, 46, 99, 115, 0, 0, 0, 1,
			0, 0, 0, 38, 92, 65, 115, 115, 101, 116,
			115, 92, 95, 83, 99, 114, 105, 112, 116, 115,
			92, 85, 73, 92, 83, 101, 116, 116, 105, 110,
			103, 115, 77, 97, 110, 97, 103, 101, 114, 46,
			99, 115, 0, 0, 0, 1, 0, 0, 0, 39,
			92, 65, 115, 115, 101, 116, 115, 92, 95, 83,
			99, 114, 105, 112, 116, 115, 92, 85, 73, 92,
			83, 116, 97, 116, 115, 86, 105, 101, 119, 77,
			97, 110, 97, 103, 101, 114, 46, 99, 115, 0,
			0, 0, 1, 0, 0, 0, 40, 92, 65, 115,
			115, 101, 116, 115, 92, 95, 83, 99, 114, 105,
			112, 116, 115, 92, 85, 73, 92, 84, 111, 111,
			108, 116, 105, 112, 67, 111, 110, 116, 114, 111,
			108, 108, 101, 114, 46, 99, 115, 0, 0, 0,
			1, 0, 0, 0, 38, 92, 65, 115, 115, 101,
			116, 115, 92, 95, 83, 99, 114, 105, 112, 116,
			115, 92, 85, 73, 92, 84, 117, 116, 111, 114,
			105, 97, 108, 77, 97, 110, 97, 103, 101, 114,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			32, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 85, 73,
			92, 85, 73, 77, 97, 110, 97, 103, 101, 114,
			46, 99, 115, 0, 0, 0, 1, 0, 0, 0,
			34, 92, 65, 115, 115, 101, 116, 115, 92, 95,
			83, 99, 114, 105, 112, 116, 115, 92, 85, 73,
			92, 90, 111, 111, 109, 67, 111, 110, 116, 101,
			110, 116, 46, 99, 115
		};
		result.TypesData = new byte[3437]
		{
			0, 0, 0, 0, 20, 124, 76, 111, 99, 97,
			108, 105, 122, 101, 100, 84, 101, 120, 116, 79,
			98, 106, 101, 99, 116, 0, 0, 0, 0, 17,
			124, 76, 111, 99, 97, 108, 105, 122, 101, 114,
			77, 97, 110, 97, 103, 101, 114, 0, 0, 0,
			0, 13, 124, 76, 68, 97, 116, 97, 70, 111,
			114, 74, 115, 111, 110, 0, 0, 0, 0, 21,
			124, 76, 111, 99, 97, 108, 105, 122, 101, 100,
			73, 110, 102, 111, 114, 109, 97, 116, 105, 111,
			110, 0, 0, 0, 0, 21, 124, 76, 111, 99,
			97, 108, 105, 122, 97, 116, 105, 111, 110, 68,
			97, 116, 97, 76, 105, 115, 116, 0, 0, 0,
			0, 24, 124, 68, 111, 117, 98, 108, 101, 65,
			110, 100, 82, 111, 117, 110, 100, 84, 111, 78,
			101, 97, 114, 101, 115, 116, 0, 0, 0, 0,
			13, 124, 83, 116, 101, 97, 109, 77, 97, 110,
			97, 103, 101, 114, 0, 0, 0, 0, 14, 124,
			116, 101, 115, 116, 65, 114, 99, 104, 101, 114,
			65, 105, 109, 0, 0, 0, 0, 10, 124, 65,
			122, 114, 97, 114, 73, 110, 102, 111, 0, 0,
			0, 0, 15, 124, 65, 122, 114, 97, 114, 67,
			111, 110, 100, 105, 116, 105, 111, 110, 0, 0,
			0, 0, 15, 124, 67, 111, 110, 100, 95, 80,
			114, 101, 118, 65, 122, 114, 97, 114, 0, 0,
			0, 0, 17, 124, 67, 111, 110, 100, 95, 80,
			108, 97, 121, 101, 114, 76, 101, 118, 101, 108,
			0, 0, 0, 0, 19, 124, 67, 111, 110, 100,
			95, 77, 111, 110, 115, 116, 101, 114, 115, 76,
			101, 118, 101, 108, 0, 0, 0, 0, 19, 124,
			67, 111, 110, 100, 95, 84, 111, 116, 97, 108,
			67, 117, 114, 114, 101, 110, 99, 121, 0, 0,
			0, 0, 15, 124, 67, 111, 110, 100, 95, 65,
			108, 119, 97, 121, 115, 77, 101, 116, 0, 0,
			0, 0, 20, 124, 67, 111, 110, 100, 95, 83,
			121, 115, 116, 101, 109, 85, 110, 108, 111, 99,
			107, 101, 100, 0, 0, 0, 0, 19, 124, 67,
			111, 110, 100, 95, 83, 107, 105, 108, 108, 85,
			110, 108, 111, 99, 107, 101, 100, 0, 0, 0,
			0, 13, 124, 65, 122, 114, 97, 114, 77, 97,
			110, 97, 103, 101, 114, 0, 0, 0, 0, 12,
			124, 65, 122, 114, 97, 114, 83, 101, 108, 102,
			101, 114, 0, 0, 0, 0, 16, 124, 67, 104,
			97, 114, 97, 99, 116, 101, 114, 83, 101, 108,
			102, 101, 114, 0, 0, 0, 0, 18, 124, 67,
			104, 97, 114, 97, 99, 116, 101, 114, 83, 116,
			97, 116, 73, 110, 102, 111, 0, 0, 0, 0,
			19, 124, 67, 104, 97, 114, 97, 99, 116, 101,
			114, 85, 73, 77, 97, 110, 97, 103, 101, 114,
			0, 0, 0, 0, 15, 124, 69, 110, 101, 109,
			105, 101, 115, 77, 97, 110, 97, 103, 101, 114,
			0, 0, 0, 0, 10, 124, 69, 110, 101, 109,
			121, 73, 110, 102, 111, 0, 0, 0, 0, 12,
			124, 69, 110, 101, 109, 121, 83, 101, 108, 102,
			101, 114, 0, 0, 0, 0, 10, 124, 83, 104,
			105, 110, 121, 73, 110, 102, 111, 0, 0, 0,
			0, 13, 124, 83, 104, 105, 110, 121, 77, 97,
			110, 97, 103, 101, 114, 0, 0, 0, 0, 23,
			124, 67, 114, 111, 115, 115, 80, 108, 97, 116,
			102, 111, 114, 109, 70, 117, 110, 99, 116, 105,
			111, 110, 115, 0, 0, 0, 0, 16, 124, 68,
			97, 116, 97, 98, 97, 115, 101, 77, 97, 110,
			97, 103, 101, 114, 0, 0, 0, 0, 11, 124,
			68, 97, 109, 97, 103, 101, 68, 97, 116, 97,
			0, 0, 0, 0, 9, 124, 69, 113, 117, 97,
			116, 105, 111, 110, 0, 0, 0, 0, 16, 124,
			70, 117, 110, 99, 116, 105, 111, 110, 115, 78,
			101, 101, 100, 101, 100, 0, 0, 0, 0, 17,
			124, 84, 104, 114, 101, 97, 100, 83, 97, 102,
			101, 82, 97, 110, 100, 111, 109, 0, 0, 0,
			0, 13, 124, 77, 121, 69, 120, 116, 101, 110,
			115, 105, 111, 110, 115, 0, 0, 0, 0, 7,
			124, 84, 119, 111, 73, 110, 116, 0, 0, 0,
			0, 23, 83, 121, 115, 116, 101, 109, 124, 79,
			98, 106, 101, 99, 116, 69, 120, 116, 101, 110,
			115, 105, 111, 110, 115, 0, 0, 0, 0, 32,
			83, 121, 115, 116, 101, 109, 124, 82, 101, 102,
			101, 114, 101, 110, 99, 101, 69, 113, 117, 97,
			108, 105, 116, 121, 67, 111, 109, 112, 97, 114,
			101, 114, 0, 0, 0, 0, 38, 83, 121, 115,
			116, 101, 109, 46, 65, 114, 114, 97, 121, 69,
			120, 116, 101, 110, 115, 105, 111, 110, 115, 124,
			65, 114, 114, 97, 121, 69, 120, 116, 101, 110,
			115, 105, 111, 110, 115, 0, 0, 0, 0, 36,
			83, 121, 115, 116, 101, 109, 46, 65, 114, 114,
			97, 121, 69, 120, 116, 101, 110, 115, 105, 111,
			110, 115, 124, 65, 114, 114, 97, 121, 84, 114,
			97, 118, 101, 114, 115, 101, 0, 0, 0, 0,
			11, 124, 69, 110, 117, 109, 72, 101, 108, 112,
			101, 114, 0, 0, 0, 0, 25, 124, 70, 117,
			110, 99, 116, 105, 111, 110, 115, 79, 102, 84,
			104, 71, 97, 109, 101, 77, 97, 110, 97, 103,
			101, 114, 0, 0, 0, 0, 17, 124, 77, 97,
			110, 97, 103, 101, 114, 79, 102, 84, 104, 101,
			71, 97, 109, 101, 0, 0, 0, 0, 13, 124,
			79, 98, 106, 101, 99, 116, 80, 111, 111, 108,
			101, 114, 0, 0, 0, 0, 11, 124, 112, 108,
			97, 121, 101, 114, 68, 97, 116, 97, 0, 0,
			0, 0, 13, 124, 83, 116, 114, 105, 110, 103,
			79, 98, 106, 101, 99, 116, 0, 0, 0, 0,
			10, 124, 73, 110, 116, 83, 116, 114, 105, 110,
			103, 0, 0, 0, 0, 14, 124, 73, 110, 116,
			76, 105, 115, 116, 83, 116, 114, 105, 110, 103,
			0, 0, 0, 0, 10, 124, 73, 110, 116, 82,
			97, 114, 105, 116, 121, 0, 0, 0, 0, 10,
			124, 82, 97, 114, 105, 116, 121, 73, 110, 116,
			0, 0, 0, 0, 13, 124, 83, 116, 114, 105,
			110, 103, 68, 111, 117, 98, 108, 101, 0, 0,
			0, 0, 11, 124, 83, 116, 114, 105, 110, 103,
			66, 111, 111, 108, 0, 0, 0, 0, 15, 124,
			83, 116, 114, 105, 110, 103, 66, 111, 111, 108,
			76, 105, 115, 116, 0, 0, 0, 0, 12, 124,
			83, 116, 114, 105, 110, 103, 70, 108, 111, 97,
			116, 0, 0, 0, 0, 10, 124, 83, 116, 114,
			105, 110, 103, 73, 110, 116, 0, 0, 0, 0,
			8, 124, 73, 110, 116, 66, 111, 111, 108, 0,
			0, 0, 0, 7, 124, 73, 110, 116, 73, 110,
			116, 0, 0, 0, 0, 9, 124, 73, 110, 116,
			70, 108, 111, 97, 116, 0, 0, 0, 0, 10,
			124, 73, 110, 116, 68, 111, 117, 98, 108, 101,
			0, 0, 0, 0, 11, 124, 73, 110, 116, 76,
			105, 115, 116, 73, 110, 116, 0, 0, 0, 0,
			12, 124, 73, 110, 116, 76, 105, 115, 116, 66,
			111, 111, 108, 0, 0, 0, 0, 14, 124, 73,
			110, 116, 76, 105, 115, 116, 82, 97, 114, 105,
			116, 121, 0, 0, 0, 0, 17, 124, 83, 116,
			114, 105, 110, 103, 76, 105, 115, 116, 82, 97,
			114, 105, 116, 121, 0, 0, 0, 0, 20, 124,
			73, 110, 116, 68, 105, 99, 116, 83, 116, 114,
			105, 110, 103, 68, 111, 117, 98, 108, 101, 0,
			0, 0, 0, 14, 124, 83, 116, 114, 105, 110,
			103, 76, 105, 115, 116, 73, 110, 116, 0, 0,
			0, 0, 14, 124, 73, 110, 116, 76, 105, 115,
			116, 68, 111, 117, 98, 108, 101, 0, 0, 0,
			0, 23, 124, 73, 110, 116, 73, 110, 116, 68,
			105, 99, 116, 83, 116, 114, 105, 110, 103, 68,
			111, 117, 98, 108, 101, 0, 0, 0, 0, 19,
			124, 73, 110, 116, 68, 105, 99, 116, 83, 116,
			114, 105, 110, 103, 70, 108, 111, 97, 116, 0,
			0, 0, 0, 17, 124, 83, 116, 114, 105, 110,
			103, 83, 116, 97, 116, 115, 70, 108, 111, 97,
			116, 0, 0, 0, 0, 17, 124, 67, 117, 114,
			114, 101, 110, 99, 105, 101, 115, 68, 111, 117,
			98, 108, 101, 0, 0, 0, 0, 22, 124, 85,
			110, 108, 111, 99, 107, 97, 98, 108, 101, 83,
			121, 115, 116, 101, 109, 115, 66, 111, 111, 108,
			0, 0, 0, 0, 16, 124, 73, 110, 116, 71,
			101, 109, 83, 97, 118, 101, 100, 68, 97, 116,
			97, 0, 0, 0, 0, 16, 124, 83, 97, 118,
			101, 76, 111, 97, 100, 77, 97, 110, 97, 103,
			101, 114, 0, 0, 0, 0, 23, 124, 83, 101,
			114, 105, 97, 108, 105, 122, 97, 98, 108, 101,
			68, 105, 99, 116, 105, 111, 110, 97, 114, 121,
			0, 0, 0, 0, 34, 83, 101, 114, 105, 97,
			108, 105, 122, 97, 98, 108, 101, 68, 105, 99,
			116, 105, 111, 110, 97, 114, 121, 124, 80, 114,
			105, 109, 101, 72, 101, 108, 112, 101, 114, 0,
			0, 0, 0, 33, 83, 101, 114, 105, 97, 108,
			105, 122, 97, 98, 108, 101, 68, 105, 99, 116,
			105, 111, 110, 97, 114, 121, 124, 69, 110, 117,
			109, 101, 114, 97, 116, 111, 114, 0, 0, 0,
			0, 22, 124, 83, 116, 97, 116, 101, 79, 102,
			84, 104, 101, 71, 97, 109, 101, 77, 97, 110,
			97, 103, 101, 114, 0, 0, 0, 0, 13, 124,
			65, 117, 116, 111, 109, 97, 116, 111, 114, 66,
			111, 116, 0, 0, 0, 0, 14, 124, 70, 105,
			110, 105, 115, 104, 77, 97, 110, 97, 103, 101,
			114, 0, 0, 0, 0, 11, 124, 82, 117, 110,
			77, 97, 110, 97, 103, 101, 114, 0, 0, 0,
			0, 9, 124, 87, 101, 108, 108, 73, 110, 102,
			111, 0, 0, 0, 0, 12, 124, 87, 101, 108,
			108, 77, 97, 110, 97, 103, 101, 114, 0, 0,
			0, 0, 12, 124, 71, 101, 109, 115, 77, 97,
			110, 97, 103, 101, 114, 0, 0, 0, 0, 12,
			124, 71, 101, 109, 83, 116, 97, 116, 73, 110,
			102, 111, 0, 0, 0, 0, 13, 124, 71, 101,
			109, 83, 97, 118, 101, 100, 68, 97, 116, 97,
			0, 0, 0, 0, 17, 124, 73, 110, 118, 101,
			110, 116, 111, 114, 121, 77, 97, 110, 97, 103,
			101, 114, 0, 0, 0, 0, 14, 124, 83, 97,
			118, 101, 100, 73, 116, 101, 109, 68, 97, 116,
			97, 0, 0, 0, 0, 14, 124, 73, 116, 101,
			109, 83, 116, 97, 116, 115, 73, 110, 102, 111,
			0, 0, 0, 0, 9, 124, 73, 116, 101, 109,
			83, 116, 97, 116, 0, 0, 0, 0, 10, 124,
			76, 101, 118, 101, 108, 73, 110, 102, 111, 0,
			0, 0, 0, 20, 124, 65, 99, 104, 105, 101,
			118, 101, 109, 101, 110, 116, 115, 77, 97, 110,
			97, 103, 101, 114, 0, 0, 0, 0, 23, 124,
			70, 108, 111, 97, 116, 105, 110, 103, 78, 117,
			109, 98, 101, 114, 115, 77, 97, 110, 97, 103,
			101, 114, 0, 0, 0, 0, 10, 124, 70, 88,
			77, 97, 110, 97, 103, 101, 114, 0, 0, 0,
			0, 20, 124, 71, 114, 111, 117, 110, 100, 67,
			108, 105, 99, 107, 97, 98, 108, 101, 73, 110,
			102, 111, 0, 0, 0, 0, 23, 124, 71, 114,
			111, 117, 110, 100, 67, 108, 105, 99, 107, 97,
			98, 108, 101, 77, 97, 110, 97, 103, 101, 114,
			0, 0, 0, 0, 22, 124, 71, 114, 111, 117,
			110, 100, 67, 108, 105, 99, 107, 97, 98, 108,
			101, 83, 101, 108, 102, 101, 114, 0, 0, 0,
			0, 19, 124, 71, 114, 111, 117, 110, 100, 69,
			102, 102, 101, 99, 116, 83, 101, 108, 102, 101,
			114, 0, 0, 0, 0, 21, 124, 71, 114, 111,
			117, 110, 100, 69, 102, 102, 101, 99, 116, 115,
			77, 97, 110, 97, 103, 101, 114, 0, 0, 0,
			0, 17, 124, 67, 117, 115, 116, 111, 109, 70,
			108, 111, 97, 116, 65, 110, 100, 71, 79, 0,
			0, 0, 0, 15, 124, 76, 111, 103, 103, 105,
			110, 103, 77, 97, 110, 97, 103, 101, 114, 0,
			0, 0, 0, 9, 124, 80, 83, 83, 101, 108,
			102, 101, 114, 0, 0, 0, 0, 18, 124, 82,
			97, 110, 100, 111, 109, 65, 117, 100, 105, 111,
			83, 101, 108, 102, 101, 114, 0, 0, 0, 0,
			12, 124, 83, 111, 117, 110, 100, 83, 101, 108,
			102, 101, 114, 0, 0, 0, 0, 17, 124, 84,
			101, 120, 116, 83, 101, 108, 102, 70, 108, 111,
			97, 116, 105, 110, 103, 0, 0, 0, 0, 24,
			124, 84, 114, 97, 105, 108, 65, 100, 100, 77,
			117, 108, 116, 105, 67, 111, 110, 116, 114, 111,
			108, 108, 101, 114, 0, 0, 0, 0, 14, 124,
			68, 101, 98, 117, 102, 102, 77, 97, 110, 97,
			103, 101, 114, 0, 0, 0, 0, 14, 124, 77,
			111, 117, 115, 101, 65, 116, 116, 97, 99, 107,
			101, 114, 0, 0, 0, 0, 12, 124, 77, 111,
			117, 115, 101, 65, 116, 116, 97, 99, 107, 0,
			0, 0, 0, 14, 124, 80, 108, 97, 121, 101,
			114, 77, 97, 110, 97, 103, 101, 114, 0, 0,
			0, 0, 19, 124, 80, 114, 111, 106, 101, 99,
			116, 105, 108, 101, 66, 101, 104, 97, 118, 105,
			111, 114, 0, 0, 0, 0, 15, 124, 80, 114,
			111, 106, 101, 99, 116, 105, 108, 101, 73, 110,
			102, 111, 0, 0, 0, 0, 16, 124, 80, 114,
			111, 106, 101, 99, 116, 105, 108, 101, 77, 111,
			118, 101, 114, 0, 0, 0, 0, 17, 124, 80,
			114, 111, 106, 101, 99, 116, 105, 108, 101, 83,
			101, 108, 102, 101, 114, 0, 0, 0, 0, 19,
			124, 80, 114, 111, 106, 101, 99, 116, 105, 108,
			101, 115, 77, 97, 110, 97, 103, 101, 114, 0,
			0, 0, 0, 17, 124, 83, 107, 105, 108, 108,
			66, 97, 114, 115, 77, 97, 110, 97, 103, 101,
			114, 0, 0, 0, 0, 16, 124, 83, 107, 105,
			108, 108, 68, 101, 116, 97, 105, 108, 73, 110,
			102, 111, 0, 0, 0, 0, 23, 124, 83, 116,
			97, 116, 86, 97, 108, 117, 101, 67, 111, 115,
			116, 69, 113, 117, 97, 116, 105, 111, 110, 115,
			0, 0, 0, 0, 10, 124, 83, 107, 105, 108,
			108, 73, 110, 102, 111, 0, 0, 0, 0, 14,
			124, 83, 107, 105, 108, 108, 115, 77, 97, 110,
			97, 103, 101, 114, 0, 0, 0, 0, 16, 124,
			83, 107, 105, 108, 108, 115, 85, 73, 77, 97,
			110, 97, 103, 101, 114, 0, 0, 0, 0, 15,
			124, 69, 110, 101, 109, 121, 83, 116, 97, 116,
			115, 68, 97, 116, 97, 0, 0, 0, 0, 16,
			124, 80, 108, 97, 121, 101, 114, 83, 116, 97,
			116, 115, 68, 97, 116, 97, 0, 0, 0, 0,
			9, 124, 83, 116, 97, 116, 73, 110, 102, 111,
			0, 0, 0, 0, 10, 124, 83, 116, 97, 116,
			115, 68, 97, 116, 97, 0, 0, 0, 0, 14,
			124, 83, 116, 97, 116, 115, 86, 97, 108, 117,
			101, 73, 110, 116, 0, 0, 0, 0, 16, 124,
			83, 116, 97, 116, 115, 86, 97, 108, 117, 101,
			70, 108, 111, 97, 116, 0, 0, 0, 0, 17,
			124, 83, 116, 97, 116, 115, 86, 97, 108, 117,
			101, 68, 111, 117, 98, 108, 101, 0, 0, 0,
			0, 9, 124, 83, 116, 97, 116, 115, 73, 110,
			116, 0, 0, 0, 0, 11, 124, 83, 116, 97,
			116, 115, 70, 108, 111, 97, 116, 0, 0, 0,
			0, 12, 124, 83, 116, 97, 116, 115, 68, 111,
			117, 98, 108, 101, 0, 0, 0, 0, 10, 124,
			83, 116, 97, 116, 115, 66, 111, 111, 108, 0,
			0, 0, 0, 7, 124, 77, 105, 110, 77, 97,
			120, 0, 0, 0, 0, 25, 124, 83, 116, 97,
			116, 115, 73, 110, 116, 84, 111, 70, 108, 111,
			97, 116, 67, 111, 110, 118, 101, 114, 116, 101,
			114, 0, 0, 0, 0, 28, 124, 84, 111, 119,
			101, 114, 67, 105, 114, 99, 108, 101, 80, 114,
			111, 106, 101, 99, 116, 105, 108, 101, 83, 101,
			108, 102, 101, 114, 0, 0, 0, 0, 10, 124,
			84, 111, 119, 101, 114, 73, 110, 102, 111, 0,
			0, 0, 0, 12, 124, 84, 111, 119, 101, 114,
			83, 101, 108, 102, 101, 114, 0, 0, 0, 0,
			14, 124, 84, 111, 119, 101, 114, 115, 77, 97,
			110, 97, 103, 101, 114, 0, 0, 0, 0, 20,
			124, 69, 120, 112, 114, 101, 115, 115, 105, 111,
			110, 69, 118, 97, 108, 117, 97, 116, 111, 114,
			0, 0, 0, 0, 26, 69, 120, 112, 114, 101,
			115, 115, 105, 111, 110, 69, 118, 97, 108, 117,
			97, 116, 111, 114, 124, 80, 97, 114, 115, 101,
			114, 1, 0, 0, 0, 12, 124, 84, 114, 101,
			101, 67, 114, 101, 97, 116, 111, 114, 1, 0,
			0, 0, 12, 124, 84, 114, 101, 101, 67, 114,
			101, 97, 116, 111, 114, 0, 0, 0, 0, 25,
			84, 114, 101, 101, 67, 114, 101, 97, 116, 111,
			114, 124, 78, 111, 100, 101, 82, 101, 102, 101,
			114, 101, 110, 99, 101, 1, 0, 0, 0, 12,
			124, 84, 114, 101, 101, 67, 114, 101, 97, 116,
			111, 114, 1, 0, 0, 0, 12, 124, 84, 114,
			101, 101, 67, 114, 101, 97, 116, 111, 114, 1,
			0, 0, 0, 12, 124, 84, 114, 101, 101, 67,
			114, 101, 97, 116, 111, 114, 1, 0, 0, 0,
			12, 124, 84, 114, 101, 101, 67, 114, 101, 97,
			116, 111, 114, 1, 0, 0, 0, 12, 124, 84,
			114, 101, 101, 67, 114, 101, 97, 116, 111, 114,
			0, 0, 0, 0, 9, 124, 84, 114, 101, 101,
			73, 110, 102, 111, 0, 0, 0, 0, 9, 124,
			84, 114, 101, 101, 76, 105, 110, 107, 0, 0,
			0, 0, 12, 124, 84, 114, 101, 101, 77, 97,
			110, 97, 103, 101, 114, 0, 0, 0, 0, 9,
			124, 84, 114, 101, 101, 78, 111, 100, 101, 0,
			0, 0, 0, 15, 124, 78, 111, 100, 101, 67,
			111, 110, 110, 101, 99, 116, 105, 111, 110, 0,
			0, 0, 0, 13, 124, 84, 114, 101, 101, 78,
			111, 100, 101, 73, 110, 102, 111, 0, 0, 0,
			0, 10, 124, 84, 114, 101, 101, 83, 116, 97,
			116, 101, 0, 0, 0, 0, 15, 124, 85, 73,
			76, 105, 110, 101, 82, 101, 110, 100, 101, 114,
			101, 114, 0, 0, 0, 0, 10, 124, 66, 97,
			114, 83, 101, 108, 102, 101, 114, 0, 0, 0,
			0, 9, 124, 66, 114, 101, 97, 116, 104, 101,
			114, 0, 0, 0, 0, 26, 124, 66, 117, 116,
			116, 111, 110, 80, 114, 101, 115, 115, 101, 100,
			77, 111, 118, 101, 84, 101, 120, 116, 68, 111,
			119, 110, 0, 0, 0, 0, 15, 124, 67, 117,
			115, 116, 111, 109, 65, 110, 105, 109, 97, 116,
			111, 114, 0, 0, 0, 0, 14, 124, 72, 111,
			118, 101, 114, 65, 110, 105, 109, 97, 116, 111,
			114, 0, 0, 0, 0, 17, 124, 77, 97, 105,
			110, 77, 101, 110, 117, 115, 77, 97, 110, 97,
			103, 101, 114, 0, 0, 0, 0, 10, 124, 79,
			110, 72, 111, 118, 101, 114, 85, 73, 0, 0,
			0, 0, 15, 124, 79, 110, 72, 111, 118, 101,
			114, 85, 73, 83, 111, 117, 110, 100, 0, 0,
			0, 0, 16, 124, 83, 101, 116, 116, 105, 110,
			103, 115, 77, 97, 110, 97, 103, 101, 114, 0,
			0, 0, 0, 17, 124, 83, 116, 97, 116, 115,
			86, 105, 101, 119, 77, 97, 110, 97, 103, 101,
			114, 0, 0, 0, 0, 18, 124, 84, 111, 111,
			108, 116, 105, 112, 67, 111, 110, 116, 114, 111,
			108, 108, 101, 114, 0, 0, 0, 0, 16, 124,
			84, 117, 116, 111, 114, 105, 97, 108, 77, 97,
			110, 97, 103, 101, 114, 0, 0, 0, 0, 10,
			124, 85, 73, 77, 97, 110, 97, 103, 101, 114,
			0, 0, 0, 0, 12, 124, 90, 111, 111, 109,
			67, 111, 110, 116, 101, 110, 116
		};
		result.TotalFiles = 100;
		result.TotalTypes = 168;
		result.IsEditorOnly = false;
		return result;
	}
}
namespace System
{
	public static class ObjectExtensions
	{
		private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

		public static bool IsPrimitive(this Type type)
		{
			if (type == typeof(string))
			{
				return true;
			}
			return type.IsValueType & type.IsPrimitive;
		}

		public static object Copy(this object originalObject)
		{
			return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
		}

		private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
		{
			if (originalObject == null)
			{
				return null;
			}
			Type type = originalObject.GetType();
			if (type.IsPrimitive())
			{
				return originalObject;
			}
			if (visited.ContainsKey(originalObject))
			{
				return visited[originalObject];
			}
			if (typeof(Delegate).IsAssignableFrom(type))
			{
				return null;
			}
			object obj = CloneMethod.Invoke(originalObject, null);
			if (type.IsArray && !type.GetElementType().IsPrimitive())
			{
				Array clonedArray = (Array)obj;
				clonedArray.ForEach(delegate(Array array, int[] indices)
				{
					array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices);
				});
			}
			visited.Add(originalObject, obj);
			CopyFields(originalObject, visited, obj, type);
			RecursiveCopyBaseTypePrivateFields(originalObject, visited, obj, type);
			return obj;
		}

		private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
		{
			if (typeToReflect.BaseType != null)
			{
				RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
				CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, (FieldInfo info) => info.IsPrivate);
			}
		}

		private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
		{
			FieldInfo[] fields = typeToReflect.GetFields(bindingFlags);
			foreach (FieldInfo fieldInfo in fields)
			{
				if ((filter == null || filter(fieldInfo)) && !fieldInfo.FieldType.IsPrimitive())
				{
					object value = InternalCopy(fieldInfo.GetValue(originalObject), visited);
					fieldInfo.SetValue(cloneObject, value);
				}
			}
		}

		public static T Copy<T>(this T original)
		{
			return (T)((object)original).Copy();
		}
	}
	public class ReferenceEqualityComparer : EqualityComparer<object>
	{
		public override bool Equals(object x, object y)
		{
			return x == y;
		}

		public override int GetHashCode(object obj)
		{
			return obj?.GetHashCode() ?? 0;
		}
	}
}
namespace System.ArrayExtensions
{
	public static class ArrayExtensions
	{
		public static void ForEach(this Array array, Action<Array, int[]> action)
		{
			if (array.LongLength != 0L)
			{
				ArrayTraverse arrayTraverse = new ArrayTraverse(array);
				do
				{
					action(array, arrayTraverse.Position);
				}
				while (arrayTraverse.Step());
			}
		}
	}
	internal class ArrayTraverse
	{
		public int[] Position;

		private int[] maxLengths;

		public ArrayTraverse(Array array)
		{
			maxLengths = new int[array.Rank];
			for (int i = 0; i < array.Rank; i++)
			{
				maxLengths[i] = array.GetLength(i) - 1;
			}
			Position = new int[array.Rank];
		}

		public bool Step()
		{
			for (int i = 0; i < Position.Length; i++)
			{
				if (Position[i] < maxLengths[i])
				{
					Position[i]++;
					for (int j = 0; j < i; j++)
					{
						Position[j] = 0;
					}
					return true;
				}
			}
			return false;
		}
	}
}
