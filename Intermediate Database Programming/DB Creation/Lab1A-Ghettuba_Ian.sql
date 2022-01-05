Drop SEQUENCE Seq_Order;
Drop SEQUENCE Seq_Item;
Drop SEQUENCE Seq_Invoice;
Drop SEQUENCE Seq_Customer;


Drop Table Rental_Invoice_Detail;
Drop Table Rental_Invoice;
Drop Table Rental_Item_Copy;
Drop Table Rental_Item;
Drop Table Special_Order_Detail;
Drop Table Special_Order_Item;
Drop Table Special_Order;
Drop Table Supplier;
Drop Table Customer;


Create Table Customer
(
	Customer_Number		NUMBER(6,0)		Constraint PK_Customer_CustomerNo Primary Key
										Constraint NN_Customer_CustomerNo Not Null,
										
	First_Name			VARCHAR2(25)	Constraint NN_Customer_FirstName Not Null,
		
	Last_Name			VARCHAR2(30)	Constraint NN_Customer_LastName Not Null,
	
	Area_Code			NUMBER(3,0)		Default '780'
										Constraint CK_Customer_AreaCode 
											Check ( REGEXP_Like(Area_Code,'[0-9][0-9][0-9]'))
										Constraint NL_Customer_Areacode Null,
	
	Phone_Number		NUMBER(7,0)		Constraint CK_Customer_Phone 
											Check ( REGEXP_Like(Phone_Number,'[0-9]{7}'))
										Constraint NL_Customer_Phone Null,
	
	Street_Address		VARCHAR2(35)	Constraint NL_Customer_Address Null,
	
	City				VARCHAR2(25)	Constraint NL_Customer_City Null,

	Province			CHAR(2)			Default 'AB'
										Constraint NL_Customer_Prov Null
										Constraint CK_Customer_Prov 
											Check ( REGEXP_Like(Province,'[A-Z][A-Z]')),
											
	Postal_Code			CHAR(6)			Constraint NL_Customer_PCode Null,
	
	Email_Address		VARCHAR2(75)	Constraint NN_Customer_Email Not Null,	
	
	Membership_Date		Date			Default sysdate
										Constraint NN_Customer_MemberDate Not Null,
	
	Current_Status_YN	CHAR(1)			Default 'Y'
										Constraint CK_Customer_StatusYN 
											Check (Current_Status_YN in ('Y','N'))
										Constraint NN_Customer_StatusYN Not Null
);


Create Table Supplier
(
	Supplier_Number		NUMBER(4,0)		Constraint PK_Supplier_SupplierNumber Primary Key
										Constraint NN_Supplier_SupplierNumber Not Null,
	
	Supplier_Name		VARCHAR2(50)	Constraint NN_Supplier_SupplierName Not Null,

	Active_YN			CHAR(1)			Default 'Y'
										Constraint CK_Supplier_Active_YN 
											Check (Active_YN in ('Y', 'N'))
										Constraint NN_Supplier_Active_YN Not Null
);


Create Table Special_Order
(
	Order_Number		NUMBER(8,0)		Constraint PK_Order_OrderNo Primary Key
										Constraint NN_Order_OrderNo Not Null,

	Customer_Number		NUMBER(6,0)		Constraint NN_Order_CustomerNo Not Null,

	Order_Date			DATE			Default sysdate
										Constraint NN_Order_OrderDate Not NULL,
	
	Received_Date		DATE			Default sysdate
										Constraint NL_Order_ReceivedDate Null,
	
	Picked_Up_Date		DATE			Default sysdate
										Constraint NL_Order_PickedUpDate Null,
                                        
										Constraint CK_Order_PickedUpDate 
											Check (Picked_Up_Date >= Received_Date),
										
										Constraint FK_Order_CustomerNo 
											Foreign Key (Customer_Number)
												References Customer (Customer_Number)
);


Create Table Special_Order_Item
(
	Item_Number			NUMBER(6,0)		Constraint PK_OrderItem_ItemNo Primary Key
										Constraint NN_OrderItem_ItemNo Not Null,
										
	Description			VARCHAR2(80)	Constraint NN_OrderItem_Description Not Null,
	
	Item_Cost			NUMBER(7,2)		Constraint NL_OrderItem_ItemCost Null
										Constraint CK_OrderItem_ItemCost 
											Check (Item_Cost > 0)
);


Create Table Special_Order_Detail
(
	Order_Number		NUMBER(8,0)		Constraint NN_OrderDetail_OrderNo Not Null,
	
	Item_Number			NUMBER(6,0)		Constraint NN_OrderDetail_ItemNo Not Null,
	
	Quantity			NUMBER(5,0)		Constraint NN_OrderDetail_Quantity Not Null
										Constraint CK_OrderDetail_Quantity 
											Check (Quantity >= 0),
	
	Sales_Price			Number(7,2)		Constraint NN_OrderDetail_SalesPrice Not Null
										Constraint CK_OrderDetail_SalesPrice 
											Check (Sales_Price > 0),
											
										Constraint PK_OrderDetail_OrderItemNo 
											Primary Key (Order_Number, Item_Number),
										
										Constraint FK_OrderDetail_OrderNo 
											Foreign Key (Order_Number) 
												References Special_Order (Order_Number),
												
										Constraint FK_OrderDetail_ItemNo 
											Foreign Key (Item_Number) 
												References Special_Order_Item (Item_Number)
);


Create Table Rental_Item
(
	Rental_Item_Number	NUMBER(4,0)		Constraint PK_Item_ItemNo Primary Key
										Constraint NN_Item_ItemNo Not Null,
										
	Supplier_Number		NUMBER(4,0)		Constraint NN_Item_SupplierNo Not Null,
											
	Description			VARCHAR2(80)	Constraint	NN_Item_Description Not Null,
	
	Rental_Duration		NUMBER(2,0)		Constraint NN_Item_Duration Not Null
										Constraint CK_Item_Duration
											Check (Rental_Duration >= 0),
											
	Rental_Rate			NUMBER(4,2)	    Default '4.40'
                                        Constraint NN_Item_Rate Not Null
										Constraint CK_Item_Rate
											Check (Rental_Rate > 0),
										
	Overdue_Rental_Rate	NUMBER(4,2)		Default '3.83'
                                        Constraint NN_Item_OverdueRate Not Null
										Constraint CK_Item_OverdueRate
											Check (Overdue_Rental_Rate > 0),
										
										
										Constraint FK_Item_SupplierNo
											Foreign Key (Supplier_Number)
												References Supplier (Supplier_Number)
);


Create Table Rental_Item_Copy
(
	Bar_Code			CHAR(8)			Constraint PK_ItemCopy_BarCode Primary Key
										Constraint NN_ItemCopy_BarCode Not Null
										Constraint CK_ItemCopy_BarCode 
											Check (REGEXP_Like(Bar_Code,'[1-9][0-9][0-9][0-9]-[0-9][0-9][0-9]'))
											Check (SUBSTR (Bar_Code, 6 , 8) not like '000'),
											
	Rental_Item_Number	NUMBER(4,0)		Constraint NN_ItemCopy_ItemNo Not Null,
	
	Purchase_Cost		NUMBER(6,2)		Constraint NL_ItemCopy_PurchaseCost Null,
										Constraint CK_ItemCopy_PurchaseCost
											Check (Purchase_Cost > 0),
	
										Constraint FK_ItemCopy_ItemNo
											Foreign Key (Rental_Item_Number)
												References Rental_Item (Rental_Item_Number)
);


Create Table Rental_Invoice
(
	Invoice_Number		NUMBER(5,0)		Constraint PK_Invoice_InvoiceNo Primary Key
										Constraint NN__Invoice_InvoiceNo Not Null,
											
	Customer_Number		NUMBER(6,0)		Constraint NN__Invoice_CustomerNo Not Null,
	
	Invoice_Date		DATE			Default sysdate
										Constraint NN_Invoice_InvoiceDate Not Null,
										
										Constraint FK_Invoice_CustomerNo
											Foreign Key (Customer_Number)
												References Customer (Customer_Number)
);


Create Table Rental_Invoice_Detail
(
	Invoice_Number		NUMBER(5,0)		Constraint NN_InvoiceDetail_InvoiceNo  Not Null,
	
	Bar_Code			CHAR(8)			Constraint NN_InvoiceDetail_BarCode Not Null,
	
	Date_Returned		DATE			Default sysdate
										Constraint NL_InvoiceDetail_RtrnDate Null,
										
	Rental_Duration		NUMBER(2,0)		Constraint NN_InvoiceDetail_Duration Not Null,
										Constraint CK_InvoiceDetail_Duration 
											Check (Rental_Duration >= 0),
	
	Original_Rental_Rate NUMBER(4,2)	Default '4.40'
                                        Constraint NN_InvoiceDetail_OrigRate Not Null
										Constraint CK_InvoiceDetail_OrigiRate
											Check (Original_Rental_Rate > 0),
                                            
	Overdue_Paid_YN		CHAR(1)			Default 'N'
                                        Constraint NL_InvoiceDetail_OverdueYN Null
										Constraint CK_InvoiceDetail_OverdueYN 
											Check (Overdue_Paid_YN in ('Y','N')),
										
	Overdue_Rental_Rate	NUMBER(4,2)		
                                        Default '3.83'
                                        Constraint NL_InvoiceDetail_Overdue Null
										Constraint CK_InvoiceDetail_Overdue
											Check (Overdue_Rental_Rate > 0),
										
										Constraint PK_InvoiceDetail_InvBar 
											Primary Key (Invoice_Number, Bar_Code),
										Constraint FK_InvoiceDetail_InvNo
											Foreign Key (Invoice_Number)
												References Rental_Invoice (Invoice_Number),
										Constraint FK_InvoiceDetail_BarCode
											Foreign Key (Bar_Code)
												References Rental_Item_Copy (Bar_Code)
);


Create SEQUENCE Seq_Customer
	Start with 750
	INCREMENT by 10
	NOCACHE;

Create SEQUENCE Seq_Invoice
	Start with 25700
	INCREMENT by 1
	NOCACHE;
	
Create Sequence Seq_Item
	Start with 9000
	INCREMENT by 100
	NOCACHE;

Create SEQUENCE Seq_Order
	Start with 500
	INCREMENT  by 5
	NOCACHE;