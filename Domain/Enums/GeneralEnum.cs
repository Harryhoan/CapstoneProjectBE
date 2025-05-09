﻿namespace Domain.Enums
{
    //public enum GeneralEnum
    //{
    //    // Placeholder for other enums
    //}

    public enum ProjectStatusEnum
    {
        VISIBLE,
        INVISIBLE,
        DELETED
    }
    public enum TransactionStatusEnum
    {
        PENDING,
        RECEIVING,
        REFUNDED,
        TRANSFERRED,
    }
    public enum UserEnum
    {
        CUSTOMER,
        STAFF,
        ADMIN
    }
    public enum PostEnum
    {
        EXCLUSIVE,
        PRIVATE,
        PUBLIC,
        DELETED
    }
    public enum PledgeDetailEnum
    {
        PLEDGED,
        TRANSFERRED,
        REFUNDED,
        TRANSFERRING,
        REFUNDING
    }
    public enum CollaboratorEnum
    {
        ADMINISTRATOR,
        EDITOR,
        VIEWER
    }
}
