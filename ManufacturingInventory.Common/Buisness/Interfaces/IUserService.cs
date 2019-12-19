﻿using ManufacturingInventory.Common.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Buisness.Interfaces {
    public enum UserAction {
        Add,
        Edit,
        Remove,
        CheckIn,
        CheckOut,
        UserManagement
    }

    public interface IUserService {
        User CurrentUser { get; set; }
        Session CurrentSession { get; set; }
        Permission UserPermission { get; set; }
        List<UserAction> AvailableUserActions { get; }
        bool Validate(UserAction action);
        bool IsValid();
        void LogOut();
    }
}