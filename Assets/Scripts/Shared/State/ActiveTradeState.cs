using DarkRift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ActiveTradeState : IDarkRiftSerializable
{

    public int RequestingPlayerId { get; set; }
    public int RecievingPlayerId { get; set; }

    public List<InventoryItemState> RequestingOffer { get; set; }
    public List<InventoryItemState> RecievingOffer { get; set; }

    public int RequestingStandardCurrency { get; set; }
    public int RequestingPremiumCurrency { get; set; }

    public int RecievingStandardCurrency { get; set; }
    public int RecievingPremiumCurrency { get; set; }

    public bool IsVendorTrade { get; set; }

    public ActiveTradeState() { }

    public ActiveTradeState(int requestingPlayerId, int recievingPlayerId)
    {
        RequestingPlayerId = requestingPlayerId;
        RecievingPlayerId = recievingPlayerId;

        if(recievingPlayerId > -1)
        {
            IsVendorTrade = false;
        } else {
            IsVendorTrade = true;
        }

        RequestingOffer = new List<InventoryItemState>();
        RecievingOffer = new List<InventoryItemState>();
    }

    public void Deserialize(DeserializeEvent e)
    {
        RequestingPlayerId = e.Reader.ReadInt32();
        RecievingPlayerId = e.Reader.ReadInt32();
        InventoryItemState[] requestingOfferTemp = e.Reader.ReadSerializables<InventoryItemState>();
        RequestingOffer = requestingOfferTemp.ToList();
        InventoryItemState[] recievingOfferTemp = e.Reader.ReadSerializables<InventoryItemState>();
        RecievingOffer = recievingOfferTemp.ToList();
        RequestingStandardCurrency = e.Reader.ReadInt32();
        RequestingPremiumCurrency = e.Reader.ReadInt32();
        RecievingStandardCurrency = e.Reader.ReadInt32();
        RecievingPremiumCurrency = e.Reader.ReadInt32();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(RequestingPlayerId);
        e.Writer.Write(RecievingPlayerId);
        e.Writer.Write(RequestingOffer.ToArray());
        e.Writer.Write(RecievingOffer.ToArray());
        e.Writer.Write(RequestingStandardCurrency);
        e.Writer.Write(RequestingPremiumCurrency);
        e.Writer.Write(RecievingStandardCurrency);
        e.Writer.Write(RecievingPremiumCurrency);
    }

}

