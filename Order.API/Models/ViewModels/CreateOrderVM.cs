﻿namespace Order.API.Models.ViewModels
{
    public class CreateOrderVM
    {
        public string BuyerId { get; set; }
        public List<CreateOrderItemVM> OrderItems { get; set; }
    }
}
