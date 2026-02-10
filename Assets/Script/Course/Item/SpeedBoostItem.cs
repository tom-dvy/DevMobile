using UnityEngine;

[CreateAssetMenu(menuName = "DevMobile/Items/SpeedBoost")]
public class SpeedBoostItem : ItemData
{
    public float boostAmount = 20f;
    public float duration = 2f;

    public override void Activate(CarController kart)
    {
        kart.ApplySpeedBoost(boostAmount, duration); 
        Debug.Log("Turbo activé !");
    }
}