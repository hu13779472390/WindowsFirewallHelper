﻿using System;
using System.Net.NetworkInformation;
using WindowsFirewallHelper.Addresses;
using WindowsFirewallHelper.Helpers;
using NetFwTypeLib;

namespace WindowsFirewallHelper.FirewallAPIv2.Rules
{
    /// <summary>
    ///     Contains properties of a Windows Firewall with Advanced Security rule
    /// </summary>
    public class StandardRule : IRule, IEquatable<StandardRule>
    {
        internal StandardRule(INetFwRule rule)
        {
            UnderlyingObject = rule;
        }

        /// <summary>
        ///     Creates a new application rule for Windows Firewall with Advanced Security
        /// </summary>
        /// <param name="name">Name of the rule</param>
        /// <param name="filename">Address of the executable file</param>
        /// <param name="action">Action that this rule defines</param>
        /// <param name="direction">Data direction in which this rule applies to</param>
        /// <param name="profiles">The profile that this rule belongs to</param>
        public StandardRule(string name, string filename, FirewallAction action, FirewallDirection direction,
            FirewallProfiles profiles)
        {
            UnderlyingObject = (INetFwRule) Activator.CreateInstance(Type.GetTypeFromProgID(@"HNetCfg.FWRule"));
            Name = name;
            ApplicationName = filename;
            Action = action;
            Direction = direction;
            IsEnable = true;
            Profiles = profiles;
        }

        /// <summary>
        ///     Creates a new port rule for Windows Firewall with Advanced Security
        /// </summary>
        /// <param name="name">Name of the rule</param>
        /// <param name="port">Port number of the rule</param>
        /// <param name="action">Action that this rule defines</param>
        /// <param name="direction">Data direction in which this rule applies to</param>
        /// <param name="profiles">The profile that this rule belongs to</param>
        public StandardRule(string name, ushort port, FirewallAction action, FirewallDirection direction,
            FirewallProfiles profiles)
        {
            UnderlyingObject = (INetFwRule) Activator.CreateInstance(Type.GetTypeFromProgID(@"HNetCfg.FWRule"));
            Name = name;
            Action = action;
            Direction = direction;
            Protocol = FirewallProtocol.TCP;
            IsEnable = true;
            Profiles = profiles;
            if (direction == FirewallDirection.Inbound)
            {
                LocalPorts = new[] {port};
            }
            else
            {
                RemotePorts = new[] {port};
            }
        }

        /// <summary>
        ///     Returns a Boolean value indicating if these class is available in the current machine
        /// </summary>
        public static bool IsSupported => Type.GetTypeFromProgID(@"HNetCfg.FWRule") != null;

        internal INetFwRule UnderlyingObject { get; }

        /// <summary>
        ///     Gets or sets the address of the executable file that this rule is about
        /// </summary>
        public string ApplicationName
        {
            get { return UnderlyingObject.ApplicationName; }
            set { UnderlyingObject.ApplicationName = value; }
        }

        /// <summary>
        ///     Gets or sets the description string about this rule
        /// </summary>
        public string Description
        {
            get { return UnderlyingObject.Description; }
            set { UnderlyingObject.Description = value; }
        }

        /// <summary>
        ///     Gets or sets if EdgeTraversal is available with this rule
        /// </summary>
        public bool EdgeTraversal
        {
            get { return UnderlyingObject.EdgeTraversal; }
            set { UnderlyingObject.EdgeTraversal = value; }
        }

        /// <summary>
        ///     Gets or sets the rule grouping string
        /// </summary>
        public string Grouping
        {
            get { return UnderlyingObject.Grouping; }
            set { UnderlyingObject.Grouping = value; }
        }

        /// <summary>
        ///     Gets or sets the network interfaces that this rule applies to by name
        /// </summary>
        public NetworkInterface[] Interfaces
        {
            get
            {
                if (!(UnderlyingObject.Interfaces is string[]))
                {
                    return new NetworkInterface[0];
                }
                return NetworkInterfaceHelper.StringToInterfaces((string[]) UnderlyingObject.Interfaces);
            }
            set { UnderlyingObject.Interfaces = NetworkInterfaceHelper.InterfacesToString(value); }
        }

        /// <summary>
        ///     Gets or sets the network interfaces that this rule applies to by type
        /// </summary>
        public FirewallInterfaceTypes InterfaceTypes
        {
            get { return NetworkInterfaceHelper.StringToInterfaceTypes(UnderlyingObject.InterfaceTypes); }
            set { UnderlyingObject.InterfaceTypes = NetworkInterfaceHelper.InterfaceTypesToString(value); }
        }

        /// <summary>
        ///     Gets or sets the name of the service that this rule is about
        /// </summary>
        public string ServiceName
        {
            get { return UnderlyingObject.serviceName; }
            set { UnderlyingObject.serviceName = value; }
        }

        /// <summary>
        ///     Gets or sets the list of the acceptable ICMP Messages with this rule
        /// </summary>
        public InternetControlMessage[] IcmpTypesAndCodes
        {
            get { return ICMPHelper.StringToICM(UnderlyingObject.IcmpTypesAndCodes); }
            set { UnderlyingObject.IcmpTypesAndCodes = ICMPHelper.ICMToString(value); }
        }

        /// <summary>
        ///     Determines whether the specified<see cref="StandardRule" /> is equal to the current
        ///     <see cref="StandardRule" />.
        /// </summary>
        /// <param name="other"> The object to compare with the current object.</param>
        /// <returns>
        ///     true if the specified <see cref="StandardRule" /> is equal to the current<see cref="StandardRule" />;
        ///     otherwise, false.
        /// </returns>
        public virtual bool Equals(StandardRule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UnderlyingObject.Name, other.UnderlyingObject.Name) &&
                   UnderlyingObject.Profiles == other.UnderlyingObject.Profiles &&
                   UnderlyingObject.Protocol == other.UnderlyingObject.Protocol &&
                   UnderlyingObject.Action == other.UnderlyingObject.Action &&
                   UnderlyingObject.Enabled == other.UnderlyingObject.Enabled &&
                   UnderlyingObject.Direction == other.UnderlyingObject.Direction &&
                   UnderlyingObject.Interfaces == other.UnderlyingObject.Interfaces &&
                   UnderlyingObject.RemoteAddresses == other.UnderlyingObject.RemoteAddresses &&
                   UnderlyingObject.RemotePorts == other.UnderlyingObject.RemotePorts &&
                   UnderlyingObject.LocalAddresses == other.UnderlyingObject.LocalAddresses &&
                   UnderlyingObject.LocalPorts == other.UnderlyingObject.LocalPorts;
        }

        /// <summary>
        ///     Gets or sets the profiles that this rule belongs to
        /// </summary>
        public FirewallProfiles Profiles
        {
            get
            {
                return (FirewallProfiles) UnderlyingObject.Profiles &
                       (FirewallProfiles.Domain | FirewallProfiles.Private |
                        FirewallProfiles.Public);
            }
            set
            {
                UnderlyingObject.Profiles = (int) (value &
                                                   (FirewallProfiles.Domain | FirewallProfiles.Private |
                                                    FirewallProfiles.Public));
            }
        }

        /// <summary>
        ///     Gets or sets the action that this rules define
        /// </summary>
        public FirewallAction Action
        {
            get
            {
                return UnderlyingObject.Action == NET_FW_ACTION_.NET_FW_ACTION_ALLOW
                    ? FirewallAction.Allow
                    : FirewallAction.Block;
            }
            set
            {
                UnderlyingObject.Action = value == FirewallAction.Allow
                    ? NET_FW_ACTION_.NET_FW_ACTION_ALLOW
                    : NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            }
        }

        /// <summary>
        ///     Gets or sets the data direction that rule applies to
        /// </summary>
        public FirewallDirection Direction
        {
            get
            {
                return UnderlyingObject.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN
                    ? FirewallDirection.Inbound
                    : FirewallDirection.Outbound;
            }
            set
            {
                UnderlyingObject.Direction = value == FirewallDirection.Inbound
                    ? NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN
                    : NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            }
        }

        /// <summary>
        ///     Gets or sets the local ports that rule applies to
        /// </summary>
        public ushort[] LocalPorts
        {
            get { return AddressHelper.StringToPorts(UnderlyingObject.LocalPorts); }
            set { UnderlyingObject.LocalPorts = AddressHelper.PortsToString(value); }
        }

        /// <summary>
        ///     Gets or sets the protocol that rule applies to
        /// </summary>
        public FirewallProtocol Protocol
        {
            get { return new FirewallProtocol(UnderlyingObject.Protocol); }
            set { UnderlyingObject.Protocol = value.ProtocolNumber; }
        }

        /// <summary>
        ///     Gets or sets the remote ports that rule applies to
        /// </summary>
        public ushort[] RemotePorts
        {
            get { return AddressHelper.StringToPorts(UnderlyingObject.RemotePorts); }
            set { UnderlyingObject.RemotePorts = AddressHelper.PortsToString(value); }
        }

        /// <summary>
        ///     Gets or sets the local addresses that rule applies to
        /// </summary>
        public IAddress[] LocalAddresses
        {
            get { return AddressHelper.StringToAddresses(UnderlyingObject.LocalAddresses); }
            set { UnderlyingObject.LocalAddresses = AddressHelper.AddressesToString(value); }
        }


        /// <summary>
        ///     Gets or sets the name of the rule
        /// </summary>
        public string Name
        {
            get { return UnderlyingObject.Name; }
            set { UnderlyingObject.Name = value; }
        }

        /// <summary>
        ///     Gets or sets a Boolean value indicating if this rule is active
        /// </summary>
        public bool IsEnable
        {
            get { return UnderlyingObject.Enabled; }
            set { UnderlyingObject.Enabled = value; }
        }

        /// <summary>
        ///     Gets or sets the scope that this rule applies to
        /// </summary>
        public FirewallScope Scope
        {
            get
            {
                if (RemoteAddresses.Length <= 1)
                {
                    foreach (var address in RemoteAddresses)
                    {
                        if (SingleIP.Any.Equals(address))
                        {
                            return FirewallScope.All;
                        }
                        if (NetworkAddress.LocalSubnet.Equals(address))
                        {
                            return FirewallScope.LocalSubnet;
                        }
                    }
                }
                return FirewallScope.Specific;
            }
            set
            {
                switch (value)
                {
                    case FirewallScope.All:
                        RemoteAddresses = new IAddress[] {SingleIP.Any};
                        break;
                    case FirewallScope.LocalSubnet:
                        RemoteAddresses = new IAddress[] {NetworkAddress.LocalSubnet};
                        break;
                    default:
                        throw new ArgumentException("Use the RemoteAddresses property to set the exact remote addresses");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the remote addresses that rule applies to
        /// </summary>
        public IAddress[] RemoteAddresses
        {
            get { return AddressHelper.StringToAddresses(UnderlyingObject.RemoteAddresses); }
            set { UnderlyingObject.RemoteAddresses = AddressHelper.AddressesToString(value); }
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="StandardRule" />
        ///     .
        /// </summary>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="StandardRule" />;
        ///     otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="StandardRule" />. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StandardRule) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="StandardRule" />.
        /// </returns>
        public override int GetHashCode()
        {
            return UnderlyingObject?.GetHashCode() ?? 0;
        }

        /// <summary>
        ///     Compares two <see cref="StandardRule" /> objects for equality
        /// </summary>
        /// <param name="left">A <see cref="StandardRule" /> object</param>
        /// <param name="right">A <see cref="StandardRule" /> object</param>
        /// <returns>true if two sides are equal; otherwise false</returns>
        public static bool operator ==(StandardRule left, StandardRule right)
        {
            return ((object) left != null && (object) right != null && left.Equals(right)) ||
                   ((object) left == null && (object) right == null);
        }

        /// <summary>
        ///     Compares two <see cref="StandardRule" /> objects for inequality
        /// </summary>
        /// <param name="left">A <see cref="StandardRule" /> object</param>
        /// <param name="right">A <see cref="StandardRule" /> object</param>
        /// <returns>true if two sides are not equal; otherwise false</returns>
        public static bool operator !=(StandardRule left, StandardRule right)
        {
            return !(left == right);
        }
    }
}