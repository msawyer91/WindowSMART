# WindowSMART
WindowSMART and Home Server SMART are now open source!

# About WindowSMART/Home Server SMART
This project was born in 2010 when my wife bought me an HP MediaSmart Server EX490 for Christmas, 2009. A longtime geek at heart, I became an active member of MediaSmartServer.net and quickly began to realize there was a desire and need for a hard disk health monitoring tool. Home Server SMART was born!

Later on, after my wife was laid off in the summer of 2010 and decided to go back to school, I expanded upon the success of Home Server SMART. Home Server SMART for the first version of Windows Home Server, which I promised would always remain free, was rebranded as Home Server SMART Classic. HSS Classic was, and technically still is, "donate-ware." It'll never nag you to donate, but there was an option in the app to send a few bucks to my PayPal.

With the release of Windows Home Server 2011, I extended HSS Classic into Home Server SMART 2012 and WindowSMART 2012. These marched on, adding support for advanced format disks and SSDs. HSS Classic was updated on the same cadence as well. My kids were getting older. No longer toddlers, they were getting involved in more and more activities. Sports, clubs, choir, etc. Since I had a full-time job already, why would I miss out on these things for freelance software development?

Long story short, the last official release of HSS Classic, Home Server SMART, or WindowSMART was in late 2015. Oh I dabbled at the code a little bit here and there, but overall it was aging gracefully.

# Open Source
In mid 2020 I officially dissolved my LLC. It was not a pandemic victim, but a personal choice. Family is important to me, and since I was already gainfully employed, I wasn't going to take time away from them.

Home Server SMART and WindowSMART need care and feeding. Lots of it. Many new SSDs have hit the market, and HSS/WindowSMART don't know how to process NVMe drives at all. I had neither the time nor the money to acquire and test so many devices.

## A Choice
I had a choice to make. I could let Home Server SMART and WindowSMART die a quiet, unceremonious death. Or I could withdraw my copyright stake and release them here on GitHub.

I choose the latter. Here's the source in all its glory. Pick it up. Run with it. Do something amazing. If you want to work on this code here in this repo, reach out to become a collaborator. I'd love to see others jump in and do something amazing.

## Licensing and Oddities in the Code
In the HomeServerSMART2013.Components module you will find some classes that relate to licensing and trying to prevent tampering. Method names, constant names, variables, etc. that don't seem to do anything like you would expect them to. Well, here's an explanation. I was paranoid that someone would reverse engineer my code, despite using the Eazfuscator obfuscation tool, and come up with a license cracking tool. I figured method names like License.ApplyLicenseKey() would make it far too easy for someone to zero in with a cracker. So the namespace, class, method, and variable names will be really off the wall. Of course, being open source now, there really isn't a need for a licensing system. Not to mention all the copyright disclaimers and such.

Anyhow, I leave the code here for you to do what you want. Your feedback is welcome.

Have fun!
