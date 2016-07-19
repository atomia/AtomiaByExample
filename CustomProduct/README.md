# Billing Provisioning Plugin for _CustomProduct_

This example code will help you to create your own **BPP**.

## Getting Started

Things that you should change:

1. namespace should be unique
 
 Change .custom to something .unique
2. package name in transformation and in class
 
 Change CustomProduct to something like WebsiteProduct

Make sure that you cover all flows with this methods
```
* AddProduct
* RemoveProduct
* ModifyProduct
* RenewProduct
```

### Installing

After applying transformation on BillingApi Web.config add new product inside admin.
```
For provisioning select your new product name everything else is up to you.
```
Notice
```
Double check that your BPP dll is placed inside BillingApi bin folder.
```
