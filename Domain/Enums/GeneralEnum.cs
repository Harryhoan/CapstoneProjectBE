﻿namespace Domain.Enums
{
    //public enum GeneralEnum
    //{
    //    // Placeholder for other enums
    //}

    public enum ProjectStatusEnum
    {
        CREATED,
        APPROVED,
        REJECTED,
        ONGOING,
        SUCCESSFUL,
        TRANSFERRED,
        INSUFFICIENT,
        REFUNDED,
        DELETED,
        SUBMITTED
    }
    //public enum TransactionStatusEnum
    //{
    //    PENDING,
    //    RECEIVING,
    //    REFUNDED,
    //    TRANSFERRED,
    //}
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
