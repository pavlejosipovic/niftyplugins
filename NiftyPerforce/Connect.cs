using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	namespace NiftyPerforce
	{
		// Main stub that interfaces towards Visual Studio.
		public class Connect : IDTExtensibility2, IDTCommandTarget
		{
			public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) { }
			public void OnAddInsUpdate(ref Array custom) { }
			public void OnStartupComplete(ref Array custom) { }

			private Plugin m_plugin = null;
			private AutoAddDelete m_addDelete = null;
			private AutoCheckout m_autoCheckout = null;

			public Connect()
			{
			}

			public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst_, ref Array custom)
			{
				if( null != m_plugin )
					return;
					
				AddIn addIn = (AddIn)addInInst_;
				m_plugin = new Plugin((DTE2)application, "NiftyPerforce", "Aurora.NiftyPerforce.Connect");

				m_plugin.RegisterCommand(addIn, "P4Edit", "", "", "Opens the current document for edit", new Plugin.OnCommandFunction(new P4Edit().OnCommand));
				m_plugin.RegisterCommand(addIn, "P4EditAllModified", "", "", "Opens all the unsaved readonly documents for edit", new Plugin.OnCommandFunction(new P4EditModified().OnCommand));
				m_plugin.RegisterCommand(addIn, "MenuItemCmd_P4EditItem", "Item;Project;Cross Project Multi Project;Cross Project Multi Item", "P4 edit", "Opens the document for edit", new Plugin.OnCommandFunction(new P4EditItem().OnCommand));
				m_plugin.RegisterCommand(addIn, "MenuItemCmd_P4RenameItem", "Item", "P4 rename", "Renames the item", new Plugin.OnCommandFunction(new P4RenameItem().OnCommand));
				m_plugin.RegisterCommand(addIn, "MenuItemCmd_P4EditSolution", "Solution", "P4 edit", "Opens the solution for edit", new Plugin.OnCommandFunction(new P4EditSolution().OnCommand));
				m_plugin.RegisterCommand(addIn, "Configuration", "", "", "Opens the configuration dialog", new Plugin.OnCommandFunction(new NiftyConfigure().OnCommand));
				
				m_addDelete = new AutoAddDelete( (DTE2)application, m_plugin.OutputPane );
				m_autoCheckout = new AutoCheckout( (DTE2)application, m_plugin.OutputPane );
				
				DebugHook((DTE2)application);
			}

			public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
			{
				if (neededText != vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
					return;

				if (!m_plugin.CanHandleCommand(commandName))
					return;

				status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}

			public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
			{
				handled = false;
				if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault)
					return;

				handled = m_plugin.OnCommand(commandName);
			}

			public void OnBeginShutdown(ref Array custom)
			{
				//TODO: Make this thing unregister all the callbacks we've just made... gahhh... C# and destructors... 
			}
			
			// Place debug stuff here for exploration.
			public void DebugHook( DTE2 application)
			{
				/*
				 * CommandBars commandBars = (CommandBars)application.CommandBars;
				foreach( CommandBar bar in commandBars)
				{
					System.Diagnostics.Debug.WriteLine( "Bar name: " + bar.Name );
				}
				 * */
				 
			}
		}
	}
}
