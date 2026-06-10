using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 玩家信息面板——显示角色属性与状态
/// </summary>
public class PanelPlayer : MonoBehaviour
{
    private TextMeshProUGUI text_name;
    private TextMeshProUGUI text_strength;
    private TextMeshProUGUI text_agility;
    private TextMeshProUGUI text_wit;
    private TextMeshProUGUI text_charm;
    private TextMeshProUGUI text_physique;
    private TextMeshProUGUI text_luck;
    private TextMeshProUGUI text_health;
    private TextMeshProUGUI text_energy;
    private TextMeshProUGUI text_satiety;
    private TextMeshProUGUI text_money;

    private void Awake()
    {
        Transform panel = this.transform;

        text_name = panel.Find("Image_Name/Text_Name").GetComponent<TextMeshProUGUI>();

        text_strength = panel.Find("Image_Attribute/Text_Strength").GetComponent<TextMeshProUGUI>();
        text_agility = panel.Find("Image_Attribute/Text_Agility").GetComponent<TextMeshProUGUI>();
        text_wit = panel.Find("Image_Attribute/Text_Wit").GetComponent<TextMeshProUGUI>();
        text_charm = panel.Find("Image_Attribute/Text_Charm").GetComponent<TextMeshProUGUI>();
        text_physique = panel.Find("Image_Attribute/Text_Physique").GetComponent<TextMeshProUGUI>();
        text_luck = panel.Find("Image_Attribute/Text_Luck").GetComponent<TextMeshProUGUI>();

        text_health = panel.Find("Image_Status/Text_Health").GetComponent<TextMeshProUGUI>();
        text_energy = panel.Find("Image_Status/Text_Energy").GetComponent<TextMeshProUGUI>();
        text_satiety = panel.Find("Image_Status/Text_Satiety").GetComponent<TextMeshProUGUI>();

        text_money = panel.Find("Text_Money").GetComponent<TextMeshProUGUI>();

        // 绑定头像按钮——打开背包
        Button button_avatar = panel.Find("Button_Avatar").GetComponent<Button>();
        button_avatar.onClick.AddListener(OnAvatarClicked);
    }

    private void Start()
    {
        Refresh();
    }

    // 点击头像按钮，打开背包面板
    private void OnAvatarClicked()
    {
        PanelInventory panel_inventory = FindObjectOfType<PanelInventory>(true);
        if (panel_inventory != null)
        {
            panel_inventory.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 刷新角色信息显示
    /// </summary>
    public void Refresh()
    {
        CharacterData player = Player.Instance.GetCharacter();
        if (player == null)
        {
            return;
        }

        text_name.text = player.name;

        text_strength.text = "力量：" + player.attributes[AttributeTypeEnum.Strength];
        text_agility.text = "敏捷：" + player.attributes[AttributeTypeEnum.Agility];
        text_wit.text = "才智：" + player.attributes[AttributeTypeEnum.Wit];
        text_charm.text = "魅力：" + player.attributes[AttributeTypeEnum.Charm];
        text_physique.text = "体魄：" + player.attributes[AttributeTypeEnum.Physique];
        text_luck.text = "气运：" + player.attributes[AttributeTypeEnum.Luck];

        text_health.text = "健康：" + (int)player.health + "/" + (int)player.max_health;
        text_energy.text = "精力：" + (int)player.energy + "/" + (int)player.max_energy;
        text_satiety.text = "饱腹：" + player.GetFullnessStatus();

        text_money.text = "金钱：" + player.money;
    }
}
