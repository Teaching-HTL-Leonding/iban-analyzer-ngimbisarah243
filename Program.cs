
int errorType = -1;
string assignment = args[0];
bool IsBankCode,
isANumber;
int bankCode = 0;
int accountNumber = 0;

if (args[0] != "build" && args[0] != "analyze") { errorType = 0; }

else if (args[0] == "build")
{
    IsBankCode = int.TryParse(args[1], out bankCode);
    isANumber = int.TryParse(args[2], out accountNumber);
    if (IsBankCode == false && isANumber == false) { errorType = 1; }
    else if (args.Length > 3) { errorType = 2; }
    else if (args[1].Length != 4) { errorType = 4; }
    else if (args[2].Length != 6) { errorType = 5; }
    else Console.WriteLine(BuildIban(bankCode, accountNumber));
}
else if (args[0] == "analyze")
{
    if (args.Length > 2) { errorType = 2; }
    else if (args[1].Length != 15) { errorType = 6; }
    else if (args[1].Substring(3).Any(char.IsLetter)) { errorType = 7; }
    else if (args[1][args[1].Length - 1] != 7) { errorType = 3; }
    else if (args[1][0] != 'N' && args[1][1] != 'O') { errorType = 8; }
    else { string iban = args[1]; AnalyzeIban(iban, out bankCode, out accountNumber); }
}

Console.WriteLine(ErrorType(errorType));

string BuildIban(int bankCode, int accountNumber)
{
    int randomNumber= Random.Shared.Next(1,3);
    int checkSum= 0;

    if (randomNumber == 1) { checkSum = Convert.ToInt32(CalculateChecksum(bankCode, accountNumber)); }
    else { checkSum= Convert.ToInt32(StepwiseChecksumCalculation(bankCode,accountNumber)); }
    
    return "NO" + checkSum + bankCode + accountNumber + '7';
}
void AnalyzeIban(string iban, out int bankCode, out int accountNumber)
{
    bankCode = Convert.ToInt32(iban.Substring(4, 8));
    accountNumber = Convert.ToInt32(iban.Substring(9));
}
long CalculateChecksum(int bankcode, int accountNumber)
{
    return 98 - Convert.ToInt64(Convert.ToString(bankcode) + Convert.ToString(accountNumber) + "7232400") % 97;
}
long StepwiseChecksumCalculation(int bankcode, int accountNumber)
{
    string code = Convert.ToString(bankcode)+Convert.ToString(accountNumber)+"7232400";

    long r1 = Convert.ToInt64(code.Substring(0,4)) % 97;
    code = code.Substring(4);
    long r2 = Convert.ToInt64(Convert.ToString(r1)+code.Substring(0,6)) % 97;
    code= code.Substring(6);
    long r3 = Convert.ToInt64(Convert.ToString(r2) + code[0]) % 97;
    code = code.Substring(1);
    long r4 = Convert.ToInt64(Convert.ToString(r3) + code.Substring(0,4)) % 97;
    code = code.Substring(4);
    long r5 = Convert.ToInt64(Convert.ToString(r4) + code )% 97;

    return 98 - r5;
}
string ErrorType(int type)
{
    return type switch
    {
        0 => "Invalid command, must be \"build\" or \"analyze\"",
        1 => "Bank code/Acoount number must not contain letters",
        2 => "Too many arguments",
        3 => "Wrong national check digit, we currently only support \"7\"",
        4 => "Bank code has wrong length, must contain 4 digits",
        5 => "Account number has wrong length, must contain 6 digits",
        6 => "Wrong length of IBAN",
        7 => "Iban must not contain letters",
        8 => "Wrong country code, we currently only support \"NO\"",
        _ => string.Empty
    };
}