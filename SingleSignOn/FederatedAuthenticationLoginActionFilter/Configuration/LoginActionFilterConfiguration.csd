<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="41aca721-ba15-4b5c-9abd-52d99ba2e85f" namespace="Atomia.Custom.FederatedAuthenticationLoginActionFilter.Configuration" xmlSchemaNamespace="http://schemas.atomia.com/2014/11/configuration/migration" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="LoginActionFilterConfiguration" namespace="Atomia.Custom.FederatedAuthenticationLoginActionFilter.Configuration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="loginActionFilterConfiguration">
      <attributeProperties>
        <attributeProperty name="Host" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="host" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/41aca721-ba15-4b5c-9abd-52d99ba2e85f/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Url" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="url" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/41aca721-ba15-4b5c-9abd-52d99ba2e85f/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Realm" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="realm" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/41aca721-ba15-4b5c-9abd-52d99ba2e85f/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationSection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>