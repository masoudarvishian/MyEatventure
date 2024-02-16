using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DummyUI : MonoBehaviour
{
    [Header("Drink Upgrade")]
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private TMP_Text _upgradeDrinkCostText;
    [SerializeField] private TMP_Text _drinkPriceText;
    [SerializeField] private Button _drinkUpgradeBtn;
    [SerializeField] private GameObject _drinkUpgradeInfo;

    [Header("Restaurant Upgrade")]
    [SerializeField] private TMP_Text _upgradeRestaurantCostText;
    [SerializeField] private GameObject _restaurantUpgradeInfo;
    [SerializeField] private Button _restaurantUpgradeBtn;
    [SerializeField] private Button _addChefBtn;

    public TMP_Text GetCoinText() => _coinText;

    public TMP_Text GetUpgradeDrinkCostText() => _upgradeDrinkCostText;

    public TMP_Text GetDrinkPriceText() => _drinkPriceText;

    public Button GetDrinkUpgradeBtn() => _drinkUpgradeBtn;

    public GameObject GetDrinkUpgradeInfo() => _drinkUpgradeInfo;

    public TMP_Text GetUpgradeRestaurantCostText() => _upgradeRestaurantCostText;

    public GameObject GetRestaurantUpgradeInfo() => _restaurantUpgradeInfo;

    public Button GetRestaurantUpgradeBtn() => _restaurantUpgradeBtn;

    public Button GetAddChefBtn() => _addChefBtn;
}
