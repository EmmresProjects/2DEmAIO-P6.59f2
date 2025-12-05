using UnityEngine;
using Cainos.CustomizablePixelCharacter; // Added for PixelCharacter namespace
using System.Collections.Generic; // For dictionaries (unused but kept for potential)

public class PFAdvancedCustomizer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject malePrefab; // Drag "PF Pixel Character - Adventurer.prefab"
    public GameObject femalePrefab; // Drag "PF Pixel Character - Adventurer F.prefab"
    public Transform spawnPoint; // Drag "CharacterSpawn"

    [Header("Male Material Arrays (Element 0 = Default .mat; drag variants)")]
    public Material[] maleHatMaterials;
    public Material[] maleHairMaterials;
    public Material[] maleClothMaterials;
    public Material[] malePantsMaterials;
    public Material[] maleBodyMaterials; // For skin tones

    [Header("Female Material Arrays (Element 0 = Default .mat; drag variants)")]
    public Material[] femaleHatMaterials;
    public Material[] femaleHairMaterials;
    public Material[] femaleClothMaterials;
    public Material[] femalePantsMaterials;
    public Material[] femaleBodyMaterials; // For skin tones

    // Current indices (persist across gender swaps; 0 = default)
    private int currentGender = 0; // 0=Male, 1=Female
    private int currentHat = 0, currentHair = 0, currentCloth = 0, currentPants = 0, currentBody = 0;

    private GameObject currentCharacter;
    private PixelCharacter currentPixelChar; // Reference to asset's script

    void Start()
    {
        UpdateCharacter();
    }

    private void UpdateCharacter()
    {
        // Destroy old
        if (currentCharacter != null) DestroyImmediate(currentCharacter);

        // Instantiate prefab
        GameObject prefab = (currentGender == 0) ? malePrefab : femalePrefab;
        currentCharacter = Instantiate(prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        currentPixelChar = currentCharacter.GetComponent<PixelCharacter>();

        if (currentPixelChar == null) return; // Safety

        // Get gender-specific arrays
        Material[] hats = (currentGender == 0) ? maleHatMaterials : femaleHatMaterials;
        Material[] hairs = (currentGender == 0) ? maleHairMaterials : femaleHairMaterials;
        Material[] cloths = (currentGender == 0) ? maleClothMaterials : femaleClothMaterials;
        Material[] pants = (currentGender == 0) ? malePantsMaterials : femalePantsMaterials;
        Material[] bodies = (currentGender == 0) ? maleBodyMaterials : femaleBodyMaterials;

        // Set materials (if not null/default; else keep prefab's)
        if (hats.Length > 0 && hats[currentHat % hats.Length] != null)
            currentPixelChar.HatMaterial = hats[currentHat % hats.Length];
        if (hairs.Length > 0 && hairs[currentHair % hairs.Length] != null)
            currentPixelChar.HairMaterial = hairs[currentHair % hairs.Length];
        if (cloths.Length > 0 && cloths[currentCloth % cloths.Length] != null)
            currentPixelChar.ClothMaterial = cloths[currentCloth % cloths.Length];
        if (pants.Length > 0 && pants[currentPants % pants.Length] != null)
            currentPixelChar.PantsMaterial = pants[currentPants % pants.Length];
        if (bodies.Length > 0 && bodies[currentBody % bodies.Length] != null)
            currentPixelChar.BodyMaterial = bodies[currentBody % bodies.Length];
    }

    // Gender bidirectional
    public void PrevGender() { currentGender = (currentGender - 1 + 2) % 2; UpdateCharacter(); }
    public void NextGender() { currentGender = (currentGender + 1) % 2; UpdateCharacter(); }

    // Hat bidirectional
    public void PrevHat()
    {
        Material[] hats = (currentGender == 0) ? maleHatMaterials : femaleHatMaterials;
        if (hats.Length > 0) { currentHat = (currentHat - 1 + hats.Length) % hats.Length; UpdateCharacter(); }
    }
    public void NextHat()
    {
        Material[] hats = (currentGender == 0) ? maleHatMaterials : femaleHatMaterials;
        if (hats.Length > 0) { currentHat = (currentHat + 1) % hats.Length; UpdateCharacter(); }
    }

    // Hair bidirectional
    public void PrevHair()
    {
        Material[] hairs = (currentGender == 0) ? maleHairMaterials : femaleHairMaterials;
        if (hairs.Length > 0) { currentHair = (currentHair - 1 + hairs.Length) % hairs.Length; UpdateCharacter(); }
    }
    public void NextHair()
    {
        Material[] hairs = (currentGender == 0) ? maleHairMaterials : femaleHairMaterials;
        if (hairs.Length > 0) { currentHair = (currentHair + 1) % hairs.Length; UpdateCharacter(); }
    }

    // Cloth bidirectional
    public void PrevCloth()
    {
        Material[] cloths = (currentGender == 0) ? maleClothMaterials : femaleClothMaterials;
        if (cloths.Length > 0) { currentCloth = (currentCloth - 1 + cloths.Length) % cloths.Length; UpdateCharacter(); }
    }
    public void NextCloth()
    {
        Material[] cloths = (currentGender == 0) ? maleClothMaterials : femaleClothMaterials;
        if (cloths.Length > 0) { currentCloth = (currentCloth + 1) % cloths.Length; UpdateCharacter(); }
    }

    // Pants bidirectional
    public void PrevPants()
    {
        Material[] pantsArr = (currentGender == 0) ? malePantsMaterials : femalePantsMaterials;
        if (pantsArr.Length > 0) { currentPants = (currentPants - 1 + pantsArr.Length) % pantsArr.Length; UpdateCharacter(); }
    }
    public void NextPants()
    {
        Material[] pantsArr = (currentGender == 0) ? malePantsMaterials : femalePantsMaterials;
        if (pantsArr.Length > 0) { currentPants = (currentPants + 1) % pantsArr.Length; UpdateCharacter(); }
    }

    // Body/Skin bidirectional
    public void PrevBody()
    {
        Material[] bodies = (currentGender == 0) ? maleBodyMaterials : femaleBodyMaterials;
        if (bodies.Length > 0) { currentBody = (currentBody - 1 + bodies.Length) % bodies.Length; UpdateCharacter(); }
    }
    public void NextBody()
    {
        Material[] bodies = (currentGender == 0) ? maleBodyMaterials : femaleBodyMaterials;
        if (bodies.Length > 0) { currentBody = (currentBody + 1) % bodies.Length; UpdateCharacter(); }
    }
}