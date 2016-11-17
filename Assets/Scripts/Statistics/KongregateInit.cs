//using UnityEngine;
//using System.Collections;

//public class KongregateInit : MonoBehaviour
//{
//    int tries;

//    void Start()
//    {
//        tries++;

//        TrySendingStat();
//    }

//    public void TrySendingStat()
//    {
//        if(KongregateAPI.IsConnected)
//        {
//            KongregateAPI.SubmitStat(KongregateAPI.GetStatisticName(KongregateAPI.STATISTIC_NAME.TIMES_PLAYED), 1);
//        }

//        tries++;
//    }
//}
