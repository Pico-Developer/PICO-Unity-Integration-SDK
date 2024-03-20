/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.

NOTICE：All information contained herein is, and remains the property of
PICO Technology Co., Ltd. The intellectual and technical concepts
contained herein are proprietary to PICO Technology Co., Ltd. and may be
covered by patents, patents in process, and are protected by trade secret or
copyright law. Dissemination of this information or reproduction of this
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd.
*******************************************************************************/

using System;

namespace Pico.Platform.Models
{
    /// <summary>
    /// The add-on that can be purchased in the app.
    ///
    /// You can create in-app products on the PICO Developer Platform.
    /// </summary>
    public class Product
    {
        /// The description of the add-on. 
        public readonly string Description;

        /// The detailed description of the add-on. 
        public readonly string DetailDescription;

        /// The price of the add-on, which is a number string. 
        public readonly string Price;

        /// The currency required for purchasing the add-on. 
        public readonly string Currency;

        /// The name of the add-on. 
        public readonly string Name;

        /// The unique identifier of the add-on. 
        public readonly string SKU;

        /// The icon of the add-on, which is an image URL.
        public readonly string Icon;

        /// The type of the add-on 
        public readonly AddonsType AddonsType;

        /// The period type for the subscription add-on. Only valid when it's a subscription add-on.
        public readonly PeriodType PeriodType;

        /// The trial period unit for the subscription add-on. Only valid when it's a subscription add-on.
        public readonly PeriodType TrialPeriodUnit;

        /// The trial period value for the subscription add-on. Only valid when it's a subscription add-on.
        public readonly int TrialPeriodValue;

        /// The original price of the add-on, which means the price without discount.
        public readonly string OriginalPrice;

        /// The order ID of the subscription. Only valid when it's a subscription add-on.
        public readonly string OuterId;

        /// Whether the subscription is auto renewed. Only valid when it's a subscription add-on.
        public readonly bool IsContinuous;

        public Product(IntPtr o)
        {
            Description = CLIB.ppf_Product_GetDescription(o);
            DetailDescription = CLIB.ppf_Product_GetDetailDescription(o);
            Price = CLIB.ppf_Product_GetPrice(o);
            Currency = CLIB.ppf_Product_GetCurrency(o);
            Name = CLIB.ppf_Product_GetName(o);
            SKU = CLIB.ppf_Product_GetSKU(o);
            Icon = CLIB.ppf_Product_GetIcon(o);
            AddonsType = CLIB.ppf_Product_GetAddonsType(o);
            PeriodType = CLIB.ppf_Product_GetPeriodType(o);
            TrialPeriodUnit = CLIB.ppf_Product_GetTrialPeriodUnit(o);
            TrialPeriodValue = CLIB.ppf_Product_GetTrialPeriodValue(o);
            OuterId = CLIB.ppf_Product_GetOuterId(o);
            OriginalPrice = CLIB.ppf_Product_GetOriginalPrice(o);
            IsContinuous = CLIB.ppf_Product_IsContinuous(o);
        }
    }

    /// <summary>
    /// Each element is \ref Product.
    /// </summary>
    public class ProductList : MessageArray<Product>
    {
        public ProductList(IntPtr a)
        {
            var count = (int) CLIB.ppf_ProductArray_GetSize(a);
            this.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                this.Add(new Product(CLIB.ppf_ProductArray_GetElement(a, (UIntPtr) i)));
            }

            NextPageParam = CLIB.ppf_ProductArray_GetNextPageParam(a);
        }
    }


    /// <summary>
    /// The add-on that the current user has purchased.
    /// </summary>
    public class Purchase
    {
        /// The expiration time. Only valid when it's a subscription add-on.
        public readonly DateTime ExpirationTime;

        /// The grant time. Only valid when it's a subscription add-on.
        public readonly DateTime GrantTime;

        /// The ID of the purchase order. 
        public readonly string ID;

        /// The unique identifier of the add-on in the purchase order. 
        public readonly string SKU;

        /// The icon of the add-on.
        public readonly string Icon;

        /// The type of the purchased add-on.
        public readonly AddonsType AddonsType;

        /// The order ID of the subscription. Only valid when it's a subscription add-on.
        public readonly string OuterId;

        /// The current period type of subscription. Only valid when it's a subscription add-on.
        public readonly PeriodType CurrentPeriodType;

        /// The next period type of subscription. Only valid when it's a subscription add-on.
        public readonly PeriodType NextPeriodType;

        /// The next pay time of subscription. Only valid when it's a subscription add-on.
        public readonly DateTime NextPayTime;

        /// The discount info of the purchase.
        public readonly DiscountType DiscountType;

        /// The comment for the order. Developers can add order comment to a purchase. See also: \ref IAPService.LaunchCheckoutFlow3
        public readonly string OrderComment;

        public Purchase(IntPtr o)
        {
            ExpirationTime = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_Purchase_GetExpirationTime(o));
            GrantTime = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_Purchase_GetGrantTime(o));
            ID = CLIB.ppf_Purchase_GetID(o);
            SKU = CLIB.ppf_Purchase_GetSKU(o);
            Icon = CLIB.ppf_Purchase_GetIcon(o);
            AddonsType = CLIB.ppf_Purchase_GetAddonsType(o);
            OuterId = CLIB.ppf_Purchase_GetOuterId(o);
            CurrentPeriodType = CLIB.ppf_Purchase_GetCurrentPeriodType(o);
            NextPeriodType = CLIB.ppf_Purchase_GetNextPeriodType(o);
            NextPayTime = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_Purchase_GetNextPayTime(o));
            DiscountType = CLIB.ppf_Purchase_GetDiscountType(o);
            OrderComment = CLIB.ppf_Purchase_GetOrderComment(o);
        }
    }

    /// <summary>
    /// Each element is \ref Purchase.
    /// </summary>
    public class PurchaseList : MessageArray<Purchase>
    {
        public PurchaseList(IntPtr a)
        {
            var count = (int) CLIB.ppf_PurchaseArray_GetSize(a);
            this.Capacity = count;
            for (int i = 0; i < count; i++)
            {
                this.Add(new Purchase(CLIB.ppf_PurchaseArray_GetElement(a, (UIntPtr) i)));
            }

            NextPageParam = CLIB.ppf_PurchaseArray_GetNextPageParam(a);
        }
    }

    /// <summary>
    /// \ref IAPService.GetSubscriptionStatus returns the subscription status of a subscription add-on.
    /// </summary>
    public class SubscriptionStatus
    {
        /// The SKU of the add-on. SKU is the add-on's unique identifier.
        public readonly string SKU;

        /// The order ID of the subscription. Only valid when it's a subscription add-on.
        public readonly string OuterId;

        /// The start time of the subscription.
        public readonly DateTime StartTime;

        /// The end time of the subscription.
        public readonly DateTime EndTime;

        /// The period type of the subscription. 
        public readonly PeriodType PeriodType;

        /// The entitlement status of the add-on, which indicates whether the user is entitled to use the add-on.
        public readonly EntitlementStatus EntitlementStatus;

        /// If `EntitlementStatus` is `Cancel`, `CancelReason` indicates why the subscription has been canceled.
        public readonly CancelReason CancelReason;

        /// Whether the subscription is in free trial.
        public readonly bool IsFreeTrial;

        /// The next period of the subscription.
        public readonly int NextPeriod;

        public SubscriptionStatus(IntPtr o)
        {
            SKU = CLIB.ppf_SubscriptionStatus_GetSKU(o);
            OuterId = CLIB.ppf_SubscriptionStatus_GetOuterId(o);
            StartTime = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_SubscriptionStatus_GetStartTime(o));
            EndTime = TimeUtil.MilliSecondsToDateTime(CLIB.ppf_SubscriptionStatus_GetEndTime(o));
            PeriodType = CLIB.ppf_SubscriptionStatus_GetPeriodType(o);
            EntitlementStatus = CLIB.ppf_SubscriptionStatus_GetEntitlementStatus(o);
            CancelReason = CLIB.ppf_SubscriptionStatus_GetCancelReason(o);
            IsFreeTrial = CLIB.ppf_SubscriptionStatus_GetIsFreeTrial(o);
            NextPeriod = CLIB.ppf_SubscriptionStatus_GetNextPeriod(o);
        }
    }
}