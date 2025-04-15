namespace Domain.Enums
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
        TRANSFERED
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
        REFUNDED
    }
    public enum CollaboratorEnum
    {
        ADMINISTRATOR,
        EDITOR,
        VIEWER
    }
}
