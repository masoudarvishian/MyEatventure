using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DummyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private TMP_Text _upgradeDrinkCostText;
    [SerializeField] private TMP_Text _drinkPriceText;
    [SerializeField] private Button _upgradeBtn;
    [SerializeField] private GameObject _upgradeInfo;

    public TMP_Text GetCoinText() => _coinText;

    public TMP_Text GetUpgradeDrinkCostText() => _upgradeDrinkCostText;

    public TMP_Text GetDrinkPriceText() => _drinkPriceText;

    public Button GetUpgradeBtn() => _upgradeBtn;

    public GameObject GetUpgradeInfo() => _upgradeInfo;
}
