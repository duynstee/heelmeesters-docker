namespace HeelmeestersAPI.Features.Shared.Auth.Models;

public class User
{
    public int Id { get; set; }

    // LET OP: DB heet dit 'mail_adress'
    public string MailAdress { get; set; } = "";

    // DB heet dit 'password' (nog geen hash-discussie nu)
    public string Password { get; set; } = "";
}