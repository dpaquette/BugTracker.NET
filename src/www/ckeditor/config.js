/*
Copyright (c) 2003-2009, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';

	config.skin = 'office2003';

	// A simpler toolbar for BugTracker.NET
	config.toolbar_Btnet=[
		
		['Cut','Copy','Paste','PasteText','-','SpellChecker', 'Scayt','-',
		'Undo','Redo','-',
		'Find','Replace','-', 
		'Link','Unlink','-',
		'Image','Table','SpecialChar','-',
		'NumberedList','BulletedList','-',
		'About'],
		'/',
		
		['Bold','Italic','Underline','Strike','-',
		'Font','FontSize','-',
		'TextColor','BGColor','-',
		'Outdent','Indent','-',
		'JustifyLeft','JustifyCenter','JustifyRight']
		
		];
	
	
	// Comment out this line to see the full CKEditor instead of the limited
	// one I chose for you - Corey
	CKEDITOR.config.toolbar='Btnet';
	
	// Scayt is the Spell-Check-As-You-Type feature.
	// If you remove Scayt, you still might want to uncomment this line so that
	// at Firefox can at least underline the mispelled words.
	// config.disableNativeSpellChecker = false;
	
	// Turn off the elementspath plugin for BugTracker.NET
	config.plugins = config.plugins.replace( /(?:^|,)elementspath(?=,|$)/, '' );
};
