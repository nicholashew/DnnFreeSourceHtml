FreeSource_Html
===============

FreeSource_Html module is a simple HTML/Text module for DNN ([DotNetNuke](https://github.com/dnnsoftware)) that support multilingual and versioning.

This project built with [Christoc's DotNetNuke Module and Theme Development Template](https://github.com/ChrisHammond/DNNTemplates).

Getting started
---------------

Install and source packages can be downloaded from the releases page on GitHub.

Drag the module to your page and start with experimenting!

![dnnfreesourcehtml](https://user-images.githubusercontent.com/3435332/44080166-cdfd13f0-9fdd-11e8-8b93-13f52fca6fc6.gif)

Usage
-----

Once you have installed the module, you can configure the **FreeSource HTML Module Settings** from the module actions setting.
		
### Basic Settings

* Replace Tokens:

  Optional tokens can be used that get replaced dynamically during display. 

* Max length of Description in search:

  Define the max length of Description used in core search.

* Language Content Fallback:

  Enable/Disable fallback to default language if localise content does not exist.

**All tab module settings above start with Prefix `FreeSource_HtmlText_*`*

### Advanced Settings

This is an admin setting that accessible by Super User or Administrator Role only.

* Maximum Number Of Versions: 

  The maximum number of versions that are maintained for any FreeSource_Html content, 
  the default value is **5** and store in **Portal Settings** `FreeSource_Html_MaximumVersionHistory`.


* Cleanup recycled module's Html Text: 

  To allow admin cleanup unwanted contents when a FreeSource_Html module hard deleted from recycle bin, this feature added due to the module deletion will not delete the FreeSource_Html contents in the database.

### Import/Export

Admins can import or export current content via module actions. 

Exported Template File will contains contents as below:

```xml
<htmlText>
	<ModuleTitle>Title Text</ModuleTitle>
	<Content>XML Encoded Html Text</Content>
	<Summary>XML Encoded Summary Text</Summary>
</htmlText>
```

***Note** - Imported item will save as current culture code and the latest version.*

Compatibility
-------------

- DNN 8.0
- DNN 7.4.2

To do
-----

- [x] Compatible with DNN 7.x
- [ ] Implement logs

Development Project References
------------------------------
* DotNetNuke v8.0.3.5
* DotNetNuke.WebControls v2.4.0.598
* DotNetNuke.WebUtility v4.2.1.783

Contributing
------------

If you'd like to contribute, please fork the repository and make changes as you'd like. Pull requests are warmly welcome.

License
-------
FreeSource_Html is released under the MIT license.
