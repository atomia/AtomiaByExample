<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="950a8a3b-c638-4298-98a1-f91e7977acfc" namespace="Atomia.Custom.IntermediateSts.Configuration" xmlSchemaNamespace="http://atomia.com/stsconfiguration" assemblyName="Atomia.Custom.IntermediateSts" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="StsConfigurationSection" namespace="Atomia.Custom.IntermediateSts.Configuration" codeGenOptions="Singleton" xmlSectionName="stsConfigurationSection">
      <elementProperties>
        <elementProperty name="RelayingParties" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="relayingParties" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/950a8a3b-c638-4298-98a1-f91e7977acfc/RelayingParties" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="RelayingParties" namespace="Atomia.Custom.IntermediateSts.Configuration" xmlItemName="relayingParty" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/950a8a3b-c638-4298-98a1-f91e7977acfc/RelayingParty" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="RelayingParty" namespace="Atomia.Custom.IntermediateSts.Configuration">
      <attributeProperties>
        <attributeProperty name="name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/950a8a3b-c638-4298-98a1-f91e7977acfc/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="certificate" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="certificate" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/950a8a3b-c638-4298-98a1-f91e7977acfc/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="homeRealm" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="homeRealm" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/950a8a3b-c638-4298-98a1-f91e7977acfc/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>