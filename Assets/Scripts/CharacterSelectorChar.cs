using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSelectorChar", menuName = "CharacterSelectorChar", order = 0)]
public class CharacterSelectorChar : ScriptableObject
{
    public string characterName;
    public Sprite perfil;
    public Sprite front;
    public Sprite back;
    public Sprite right;
    public Sprite left;
    public AnimatorOverrideController animatorOverrideController;
}
