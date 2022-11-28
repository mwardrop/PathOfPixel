using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TradePanel : BasePanel
{
    // Start is called before the first frame update
    protected override void Start()
    {
        CloseButton.onClick.AddListener(CloseClicked);

        //base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void CloseClicked()
    {
        ClientManager.Instance.StateManager.Actions.CancelTrade();

    }
}
