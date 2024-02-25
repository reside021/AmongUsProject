using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VentsManager : MonoBehaviour
{
    public static Action<Transform> ChangeVents;


    public Transform AdminVents;
    public Transform CafeteriaVents;
    public Transform ElectricalVents;
    public Transform EngineDownVents;
    public Transform EngineUpVents;
    public Transform MedBayVents;
    public Transform NavigationUpVents;
    public Transform NavigationDownVents;
    public Transform NavHallWayVents;
    public Transform ReactorUpVents;
    public Transform ReactorDownVents;
    public Transform SecuirityVents;
    public Transform ShieldsVents;
    public Transform WeaponsVents;

    #region ButtonUI

    public Button CafeteriaToWeaponsBtn;
    public Button CafeteriaToNavHallWayBtn;
    public Button CafeteriaToMedBayBtn;

    public Button WeaponsToCafeteriaBtn;
    public Button WeaponsToNavHallWayBtn;
    public Button WeaponsToNavigationUpBtn;

    public Button NavHallWayToAdminBtn;
    public Button NavHallWayToShieldsBtn;
    public Button NavHallWayToNavigationDownBtn;
    public Button NavHallWayToWeaponsBtn;

    public Button NavigationUpToWeaponsBtn;
    public Button NavigationUpToNavigationDownBtn;
    public Button NavigationUpToNavHallWayBtn;

    public Button NavigationDownToNavigationUpBtn;
    public Button NavigationDownToNavHallWayBtn;
    public Button NavigationDownToShieldsBtn;

    public Button ShieldsToNavHallWayBtn;
    public Button ShieldsToAdminBtn;

    public Button AdminToNavHallWayBtn;
    public Button AdminToShieldsBtn;
    public Button AdminToCafeteriaBtn;
    public Button AdminToElectricalBtn;

    public Button ElecticalToAdminBtn;
    public Button ElecticalToMedBayBtn;
    public Button ElecticalToSecuirityBtn;
    public Button ElecticalToEngineDownBtn;

    public Button MedBayToSecuirityBtn;
    public Button MedBayToElectricalBtn;
    public Button MedBayToCafeteriaBtn;
    public Button MedBayToEngineUpBtn;

    public Button SecuirityToElectricalBtn;
    public Button SecuirityToMedBayBtn;
    public Button SecuirityToEngineDownBtn;
    public Button SecuirityToReactorDownBtn;

    public Button EngineDownToElectricalBtn;
    public Button EngineDownToReactorDownBtn;
    public Button EngineDownToSecuirityBtn;

    public Button EngineUpToReactorUpBtn;
    public Button EngineUpToCafeteriaBtn;
    public Button EngineUpToMedBayBtn;

    public Button ReactorUpToReactorDownBtn;
    public Button ReactorUpToEngineUpBtn;

    public Button ReactorDownToReactorUpBtn;
    public Button ReactorDownToEngineDownBtn;
    public Button ReactorDownToSecuirityBtn;

    #endregion

    void Start()
    {
        CafeteriaToWeaponsBtn.onClick.AddListener(ToWeapons);
        CafeteriaToNavHallWayBtn.onClick.AddListener(ToNavHallWay);
        CafeteriaToMedBayBtn.onClick.AddListener(ToMedBay);

        WeaponsToCafeteriaBtn.onClick.AddListener(ToCafeteria);
        WeaponsToNavHallWayBtn.onClick.AddListener(ToNavHallWay);
        WeaponsToNavigationUpBtn.onClick.AddListener(ToNavigationUp);

        NavHallWayToAdminBtn.onClick.AddListener(ToAdmin);
        NavHallWayToShieldsBtn.onClick.AddListener(ToShields);
        NavHallWayToNavigationDownBtn.onClick.AddListener(ToNavigationDown);
        NavHallWayToWeaponsBtn.onClick.AddListener(ToWeapons);

        NavigationUpToWeaponsBtn.onClick.AddListener(ToWeapons);
        NavigationUpToNavigationDownBtn.onClick.AddListener(ToNavigationDown);
        NavigationUpToNavHallWayBtn.onClick.AddListener(ToNavHallWay);

        NavigationDownToNavigationUpBtn.onClick.AddListener(ToNavigationUp);
        NavigationDownToNavHallWayBtn.onClick.AddListener(ToNavHallWay);
        NavigationDownToShieldsBtn.onClick.AddListener(ToShields);

        ShieldsToNavHallWayBtn.onClick.AddListener(ToNavHallWay);
        ShieldsToAdminBtn.onClick.AddListener(ToAdmin);

        AdminToNavHallWayBtn.onClick.AddListener(ToNavHallWay);
        AdminToShieldsBtn.onClick.AddListener(ToShields);
        AdminToCafeteriaBtn.onClick.AddListener(ToCafeteria);
        AdminToElectricalBtn.onClick.AddListener(ToElectrical);

        ElecticalToAdminBtn.onClick.AddListener(ToAdmin);
        ElecticalToMedBayBtn.onClick.AddListener(ToMedBay);
        ElecticalToSecuirityBtn.onClick.AddListener(ToSecuirity);
        ElecticalToEngineDownBtn.onClick.AddListener(ToEngineDown);

        MedBayToSecuirityBtn.onClick.AddListener(ToSecuirity);
        MedBayToElectricalBtn.onClick.AddListener(ToElectrical);
        MedBayToCafeteriaBtn.onClick.AddListener(ToCafeteria);
        MedBayToEngineUpBtn.onClick.AddListener(ToEngineUp);

        SecuirityToElectricalBtn.onClick.AddListener(ToElectrical);
        SecuirityToMedBayBtn.onClick.AddListener(ToMedBay);
        SecuirityToEngineDownBtn.onClick.AddListener(ToEngineDown);
        SecuirityToReactorDownBtn.onClick.AddListener(ToReactorDown);

        EngineDownToElectricalBtn.onClick.AddListener(ToElectrical);
        EngineDownToReactorDownBtn.onClick.AddListener(ToReactorDown);
        EngineDownToSecuirityBtn.onClick.AddListener(ToAdmin);

        EngineUpToReactorUpBtn.onClick.AddListener(ToReactorUp);
        EngineUpToCafeteriaBtn.onClick.AddListener(ToCafeteria);
        EngineUpToMedBayBtn.onClick.AddListener(ToMedBay);

        ReactorUpToReactorDownBtn.onClick.AddListener(ToReactorDown);
        ReactorUpToEngineUpBtn.onClick.AddListener(ToEngineUp);

        ReactorDownToReactorUpBtn.onClick.AddListener(ToReactorUp);
        ReactorDownToEngineDownBtn.onClick.AddListener(ToEngineDown);
        ReactorDownToSecuirityBtn.onClick.AddListener(ToSecuirity);
    }


    private void ToCafeteria()
    {
        ChangeVents?.Invoke(CafeteriaVents);
    }

    private void ToWeapons()
    {
        ChangeVents?.Invoke(WeaponsVents);
    }

    private void ToNavHallWay()
    {
        ChangeVents?.Invoke(NavHallWayVents);
    }

    private void ToNavigationUp()
    {
        ChangeVents?.Invoke(NavigationUpVents);
    }

    private void ToNavigationDown()
    {
        ChangeVents?.Invoke(NavigationDownVents);
    }
    private void ToShields()
    {
        ChangeVents?.Invoke(ShieldsVents);
    }

    private void ToAdmin()
    {
        ChangeVents?.Invoke(AdminVents);
    }

    private void ToElectrical()
    {
        ChangeVents?.Invoke(ElectricalVents);
    }
    private void ToMedBay()
    {
        ChangeVents?.Invoke(MedBayVents);
    }
    private void ToSecuirity()
    {
        ChangeVents?.Invoke(SecuirityVents);
    }
    private void ToEngineDown()
    {
        ChangeVents?.Invoke(EngineDownVents);
    }
    private void ToEngineUp()
    {
        ChangeVents?.Invoke(EngineUpVents);
    }
    private void ToReactorUp()
    {
        ChangeVents?.Invoke(ReactorUpVents);
    }
    private void ToReactorDown()
    {
        ChangeVents?.Invoke(ReactorDownVents);
    }


}
