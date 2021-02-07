fun main() {
    val content = """
    <SolidColorBrush x:Key="BackgroundColorAccent" Color="{DynamicResource ColorAccent}" />
    <SolidColorBrush x:Key="BackgroundColorAccentHover" Color="{DynamicResource ColorAccentHover}" />
    <SolidColorBrush x:Key="BackgroundColorAccentClick" Color="{DynamicResource ColorAccentClick}" />
    <SolidColorBrush x:Key="BackgroundColorPrimaryDark" Color="{DynamicResource ColorPrimaryDark}" />
    <SolidColorBrush x:Key="BackgroundColorPrimaryDarkHover" Color="{DynamicResource ColorPrimaryDarkHover}" />
    <SolidColorBrush x:Key="BackgroundColorPrimaryDarkClick" Color="{DynamicResource ColorPrimaryDarkClick}" />
    <SolidColorBrush x:Key="BackgroundColorPrimary" Color="{DynamicResource ColorPrimary}" />
    <SolidColorBrush x:Key="BackgroundColorPrimaryHover" Color="{DynamicResource ColorPrimaryHover}" />
    <SolidColorBrush x:Key="BackgroundColorPrimaryClick" Color="{DynamicResource ColorPrimaryClick}" />
    <SolidColorBrush x:Key="BackgroundColorConfirm" Color="{DynamicResource ColorConfirm}" />
    <SolidColorBrush x:Key="BackgroundColorPrimaryLight" Color="{DynamicResource ColorPrimaryLight}" />
    <SolidColorBrush x:Key="BackgroundColorTextfieldBackground" Color="{DynamicResource ColorTextfieldBackground}" />
    <SolidColorBrush x:Key="BackgroundColorTextfieldBackgroundHover" Color="{DynamicResource ColorTextfieldBackgroundHover}" />
    <SolidColorBrush x:Key="BackgroundColorTextPrimary" Color="{DynamicResource ColorTextPrimary}" />
    <SolidColorBrush x:Key="BackgroundColorTextSecondary" Color="{DynamicResource ColorTextSecondary}" />
    <SolidColorBrush x:Key="BackgroundColorTempo" Color="{DynamicResource ColorTempo}" />
    <SolidColorBrush x:Key="BackgroundColorAutoGenerate" Color="{DynamicResource ColorAutoGenerate}" />
    <SolidColorBrush x:Key="BackgroundColorAutoGenerateHover" Color="{DynamicResource ColorAutoGenerateHover}" />
    <SolidColorBrush x:Key="BackgroundColorAutoGenerateClick" Color="{DynamicResource ColorAutoGenerateClick}" />
    <SolidColorBrush x:Key="BackgroundColorKey" Color="{DynamicResource ColorKey}" />
    <SolidColorBrush x:Key="BackgroundColorLength" Color="{DynamicResource ColorLength}" />
    <SolidColorBrush x:Key="BackgroundColorGenre" Color="{DynamicResource ColorGenre}" />
    <SolidColorBrush x:Key="BackgroundColorConfirmHover" Color="{DynamicResource ColorConfirmHover}" />
    <SolidColorBrush x:Key="BackgroundColorConfirmClick" Color="{DynamicResource ColorConfirmClick}" />
    <SolidColorBrush x:Key="BackgroundColorIcon" Color="{DynamicResource ColorIcon}" />
    <Color x:Key="ColorAccent">#A00041</Color>
    <Color x:Key="ColorAccentHover">#B3024A</Color>
    <Color x:Key="ColorAccentClick">#C02564</Color>
    <Color x:Key="ColorPrimaryDark">#3B3B3B</Color>
    <Color x:Key="ColorPrimaryDarkHover">#434343</Color>
    <Color x:Key="ColorPrimaryDarkClick">#5A5959</Color>
    <Color x:Key="ColorPrimary">#4E4F4F</Color>
    <Color x:Key="ColorPrimaryHover">#5C5C5C</Color>
    <Color x:Key="ColorPrimaryClick">#656666</Color>
    <Color x:Key="ColorConfirm">#359100</Color>
    <Color x:Key="ColorPrimaryLight">#929292</Color>
    <Color x:Key="ColorTextfieldBackground">#707272</Color>
    <Color x:Key="ColorTextfieldBackgroundHover">#787B7B</Color>
    <Color x:Key="ColorTextPrimary">#FCFCFC</Color>
    <Color x:Key="ColorTextSecondary">#B8B8B8</Color>
    <Color x:Key="ColorTempo">#63CCFF</Color>
    <Color x:Key="ColorAutoGenerate">#0C92D3</Color>
    <Color x:Key="ColorAutoGenerateHover">#1FA1E0</Color>
    <Color x:Key="ColorAutoGenerateClick">#3FBBF7</Color>
    <Color x:Key="ColorKey">#A00041</Color>
    <Color x:Key="ColorLength">#67FF0F</Color>
    <Color x:Key="ColorGenre">#FFD13F</Color>
    <Color x:Key="ColorConfirmHover">#3EA502</Color>
    <Color x:Key="ColorConfirmClick">#45BD00</Color>
    <Color x:Key="ColorIcon">#9B9B9B</Color>"""
    
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