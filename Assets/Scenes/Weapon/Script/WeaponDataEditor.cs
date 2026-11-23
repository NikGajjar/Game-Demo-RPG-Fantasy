
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Item Information
        Draw("itemName");
        Draw("itemIcon");
        Draw("category");
        Draw("worldPrefab");
        Draw("stackable");

        if (serializedObject.FindProperty("stackable").boolValue)
            Draw("maxStackSize");

        EditorGUILayout.Space();

        // Weapon Information
        EditorGUILayout.LabelField("Weapon Information", EditorStyles.boldLabel);

        Draw("weaponType");
        Draw("damage");

        WeaponType type = (WeaponType)
            serializedObject.FindProperty("weaponType").enumValueIndex;

        EditorGUILayout.Space();

        // Weapon Models
        EditorGUILayout.LabelField("Weapon Models", EditorStyles.boldLabel);

        Draw("handPrefab");
        Draw("offHandPrefab");
        Draw("storedPrefab");
        Draw("secondaryStoredPrefab");

        EditorGUILayout.Space();

        // Weapon-specific settings
        switch (type)
        {
            case WeaponType.Sword:
                DrawSword();
                break;

            case WeaponType.Shield:
                DrawShield();
                break;

            case WeaponType.SwordShield:
                DrawSwordShield();
                break;

            case WeaponType.Bow:
                DrawBow();
                break;

            case WeaponType.Snowball:
            case WeaponType.Rock:
                DrawThrowable();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    //  SWORD 

    private void DrawSword()
    {
        EditorGUILayout.LabelField("Sword Animations", EditorStyles.boldLabel);

        Draw("swordUnsheatheAnimation");
        Draw("swordSheatheAnimation");
        Draw("idleAnimation");
        Draw("combatIdleAnimation");
        Draw("attackAnimations");
    }

    //  SHIELD 

    private void DrawShield()
    {
        EditorGUILayout.LabelField("Shield Animations", EditorStyles.boldLabel);

        Draw("shieldUnsheatheAnimation");
        Draw("shieldSheatheAnimation");
        Draw("idleAnimation");
        Draw("combatIdleAnimation");
        Draw("blockAnimation");
    }

    // OLD COMBINED WEAPON 

    private void DrawSwordShield()
    {
        EditorGUILayout.LabelField(
            "Sword & Shield Animations",
            EditorStyles.boldLabel
        );

        Draw("swordUnsheatheAnimation");
        Draw("shieldUnsheatheAnimation");
        Draw("swordSheatheAnimation");
        Draw("shieldSheatheAnimation");
        Draw("idleAnimation");
        Draw("combatIdleAnimation");
        Draw("attackAnimations");
        Draw("blockAnimation");
    }

    //  BOW 

    private void DrawBow()
    {
        EditorGUILayout.LabelField("Bow Projectile", EditorStyles.boldLabel);

        Draw("projectilePrefab");
        Draw("projectileSpeed");
        Draw("ammunitionData");
        Draw("ammoRequiredPerShot");

        EditorGUILayout.LabelField("Bow Animations", EditorStyles.boldLabel);

        Draw("bowUnsheatheAnimation");
        Draw("bowSheatheAnimation");
        Draw("bowIdleAnimation");
        Draw("arrowUnsheatheAnimation");
        Draw("bowCombatIdleAnimation");
        Draw("bowLoadAnimation");
        Draw("bowHoldAnimation");
        Draw("bowReleaseAnimation");

        Draw("projectilePrefab");
        Draw("projectileSpeed");
        Draw("ammunitionData");
        Draw("ammoRequiredPerShot");
        Draw("arrowHandPrefab");
        Draw("arrowUnsheatheAnimation");
    }

    //  THROWABLE 

    private void DrawThrowable()
    {
        EditorGUILayout.LabelField(
            "Throwable Projectile",
            EditorStyles.boldLabel
        );

        Draw("projectilePrefab");
        Draw("projectileSpeed");

        EditorGUILayout.LabelField(
            "Throwable Animation",
            EditorStyles.boldLabel
        );

        Draw("throwAnimation");
    }

    //  DRAW PROPERTY 

    private void Draw(string propertyName)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null) EditorGUILayout.PropertyField(property, true);
    }
}
