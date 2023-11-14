using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ShootingViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public int Ammo { get; set; }

    public CopyItem Item
    {
        get { return _shootModel.Item; }
        set { _shootModel.Item = value; }
    }

    private ShootModel _shootModel;
    private int returnAmmo;

    public ShootingViewModel(ShootModel shootModel)
    {
        _shootModel = shootModel;
        _shootModel.PropertyChanged += HandlePropertyChanged;
    }

    public void OnButtonReloadClick()
    {
        Reload();
    }

    public void OnButtonShootClick()
    {
        Shoot();
    }

    private void Shoot()
    {
        if(Ammo != 0)
        {
            Ammo -= 4;
            OnPropertyChanged(nameof(Ammo));
        }
    }

    private void Reload()
    {
        if(Item != null)
        {
            // ���������, ���� Ammo ������ ��� ����� 16, �� �������
            if (Ammo >= _shootModel.Item.CountOfBullets)
            {
                return;
            }

            int neededAmmo = _shootModel.Item.CountOfBullets - Ammo;

            // ������� ����� ������ ��� ��������
            List<IInventoryItemView> ammoSlotsCopy = new List<IInventoryItemView>(_shootModel.InventoryManager.AmmoSlots);

            // ���������� ��� �������� ���������
            foreach (var itemInSlot in ammoSlotsCopy)
            {
                if (itemInSlot.Item.Count >= neededAmmo)
                {
                    itemInSlot.Item.Count -= neededAmmo;
                    AddAmmo(neededAmmo);
                    itemInSlot.RefreshCount();
                    break;
                }
                else
                {
                    int takenAmmo = itemInSlot.Item.Count;
                    itemInSlot.Item.Count = 0;
                    AddAmmo(takenAmmo);
                    neededAmmo -= takenAmmo;

                    itemInSlot.RefreshCount();
                }
            }
        }
    }

    private void AddAmmo(int amount)
    {
        Ammo += amount;
        OnPropertyChanged(nameof(Ammo));
    }

    private void RemoveAmmo(int amount)
    {
        Ammo -= amount;
        OnPropertyChanged(nameof(Ammo));
    }

    private void ChangeWeapon()
    {
        if (Ammo >= _shootModel.Item.CountOfBullets)
        {
            returnAmmo = Ammo - _shootModel.Item.CountOfBullets;
            RemoveAmmo(returnAmmo);
        }

        List<IInventoryItemView> ammoSlotsCopy = new List<IInventoryItemView>(_shootModel.InventoryManager.AmmoSlots);

        // ���������� ��� �������� ���������
        foreach (var itemInSlot in ammoSlotsCopy)
        {
            itemInSlot.Item.Count += returnAmmo;
            itemInSlot.RefreshCount();
            break;
        }
    }

    private void ReturnAmmoToInventory()
    {
        List<IInventoryItemView> ammoSlotsCopy = new List<IInventoryItemView>(_shootModel.InventoryManager.AmmoSlots);

        // ���������� ��� �������� ���������
        foreach (var itemInSlot in ammoSlotsCopy)
        {
            itemInSlot.Item.Count += Ammo;
            itemInSlot.RefreshCount();
            RemoveAmmo(Ammo);
            break;  // ����� �������� ������� ������ � ������ ���������� ������
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_shootModel.Item):
                if(_shootModel.Item != null)
                {
                    OnPropertyChanged(nameof(Item));
                    ChangeWeapon();
                }
                else
                {
                    OnPropertyChanged(nameof(Item));
                    ReturnAmmoToInventory();
                }
                break;
        }
    }
}
