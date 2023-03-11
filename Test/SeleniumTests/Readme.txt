



https://stackoverflow.com/questions/48734097/the-name-pagefactory-does-not-exist-in-the-current-context

Namely, with 3.11.0 release of Selenium.Support, PageFactory and ExpectedConditions were marked as obsolete. With Selenium.Support 3.12.0 they have been removed completely. More on that topic here.

Solution to this is to simply add DotNetSeleniumExtras to your packages as those were moved to separate repository. One can also find useful Dreamescaper's fork (NuGet) that has added .NET Core support until the original repo finds an owner.