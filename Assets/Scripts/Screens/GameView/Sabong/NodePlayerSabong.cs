using System.Collections;
using System.Collections.Generic;
using Globals;
using UnityEngine;

public class NodePlayerSabong : BaseView
{
    private static NodePlayerSabong _Instance;
    private NodePlayerSabong() { }

    [SerializeField] private ItemPlayerSabong m_PrefabPlayerIPS;
    [SerializeField] private Transform m_PlayersTf;

    #region Button
    public void DoClickClose()
    {
        onClickClose(false);
    }
    #endregion

    private void Init(List<Player> DataPs)
    {
        foreach (Transform tf in m_PlayersTf.transform) Destroy(tf.gameObject);
        for (int i = 0; i < DataPs.Count; i++)
        {
            ItemPlayerSabong ips = Instantiate(m_PrefabPlayerIPS, m_PlayersTf);

            ips.SetData(DataPs[i]);
            ips.gameObject.SetActive(true);
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        _Instance = null;
    }
    public static NodePlayerSabong Show(NodePlayerSabong prefab, Transform parentTf, List<Player> players)
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(prefab, parentTf);
            _Instance.transform.localScale = Vector3.one;
            _Instance.transform.localPosition = Vector3.zero;

        }
        else
        {
            _Instance.gameObject.SetActive(true);
        }
        _Instance.Init(players);
        return _Instance;
    }
}
