using IdolTower.Data;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Create GameConfigDataContainer", fileName = "GameConfigDataContainer", order = 0)]

    public class GameConfigDataContainer : DataContainer<GameConfigData>
    {
    }
}