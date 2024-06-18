using System.Text;

public static class CsvData
{
    public static readonly string Text1 =
@"Name,Age
Katie Davis,58
Alex Brown,77
Alex Taylor,52
Jane Williams,35
Sarah Williams,18
David Moore,60
John Brown,46
Katie Smith,73
Laura Williams,78
Michael Moore,42
Alex Davis,54
Chris Williams,73
Michael Davis,77
Alex Smith,20
David Wilson,70
Chris Jones,63
Michael Taylor,27
Jane Brown,59
David Wilson,33
Sarah Davis,22
John Smith,60
Sarah Taylor,34
Katie Smith,34
Chris Moore,56
Alex Brown,49
Laura Davis,61
Emily Williams,49
Sarah Miller,73
Sarah Brown,18
Katie Brown,70
Chris Brown,69
Laura Miller,69
Katie Smith,34
Jane Brown,45
Laura Miller,18
Michael Taylor,57
Chris Wilson,46
Katie Davis,74
Alex Smith,18
Katie Brown,70
Emily Williams,25
Katie Johnson,19
Sarah Brown,50
Emily Jones,58
Emily Jones,39
David Miller,75
Emily Smith,78
Emily Johnson,42
Emily Davis,65
Laura Wilson,28
Laura Davis,59
Jane Smith,52
Alex Taylor,78
Alex Wilson,56
Laura Taylor,80
Katie Wilson,69
Sarah Miller,73
Laura Brown,22
Jane Taylor,67
Emily Johnson,64
Katie Miller,18
Laura Williams,79
Emily Moore,55
Laura Williams,62
Katie Miller,61
Katie Smith,71
Michael Moore,35
Laura Wilson,48
Michael Brown,46
Laura Miller,65
David Johnson,30
Sarah Miller,57
Jane Miller,80
Alex Williams,77
Emily Williams,22
David Johnson,36
Sarah Moore,78
Laura Brown,55
Sarah Moore,78
Michael Wilson,62
Alex Taylor,56
John Smith,64
Emily Taylor,55
Sarah Johnson,76
Michael Smith,30
David Williams,69
Emily Smith,25
Chris Brown,61
Laura Williams,64
Alex Smith,31
David Miller,53
David Johnson,24
David Brown,64
Chris Johnson,27
Jane Jones,78
Emily Taylor,19
Katie Taylor,32
Jane Taylor,47
Emily Wilson,24
Emily Davis,75";

    public static readonly string Text2 =
@"Alpha,Beta,Gamma
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3
1,2,3";

    public static readonly byte[] Utf8Text1 = Encoding.UTF8.GetBytes(Text1);
    public static readonly byte[] Utf8Text2 = Encoding.UTF8.GetBytes(Text2);
}