fun main() {
    val content = """<system:String x:Key="StrTemplateSetlistExportSuccess">Die Setlist wurde erfolgreich exportiert. Sie ist unter # vorzufinden</system:String>
 <system:String x:Key="StrExportSetlistFailed">Das Exportieren dieser Setlist ist fehlgeschlagen. Bitte versuchen Sie es erneut!</system:String>
"""
    
    content.split("\n")
        .map{it.trim()}
        .filter{it.isNotEmpty()}
        .map{
            val firstHyphenIdx = it.indexOf("\u0022")
            val secondtHyphenIdx = it.indexOf("\u0022",firstHyphenIdx+1)
            it.substring(firstHyphenIdx+1,secondtHyphenIdx);
        }
        .map{
            "public const string ${it} = \u0022${it}\u0022;"
        }
        .forEach{
            println(it)
        }
}