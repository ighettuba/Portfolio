
--QUESTION 3	
			--PROCEDURES
Create or replace procedure PR_Q3
	(P_First VARCHAR2, P_Last VARCHAR2)
authid current_user
AS
--variables
	V_Customer NUMBER(6,0);
	V_Output VARCHAR2(500);
BEGIN
--Search for Customer record in DB
	Select 	Customer_Number
	Into 	V_Customer
	From 	Customer
	Where 	upper(First_Name) = upper(P_First) --change to uppercase
			and upper(Last_Name) = upper(P_Last)
			and Current_Status_YN = 'Y';
--Insert record
	Insert Into Rental_Invoice
		(Invoice_Number, Customer_Number, Invoice_Date)
	Values
		(Seq_Invoice.NEXTVAL, V_Customer, sysdate);	
    Exception
    When No_Data_Found Then
        Raise_Application_Error(-20001,'There are no customers by that name...');
    When Too_Many_Rows Then
        Raise_Application_Error(-20002,'There are multiple customers with that name...');
	When Others THEN
		Raise_Application_Error(-20404,'Please contact tech support at (780)935-9404...');
END PR_Q3;
/
Show Errors;

--QUESTION 4
				--FUNCTIONS
		--Package Specification
Create or Replace Package PKG_Q4
authid current_user
IS	
	Function FN_Q4
		(P_First VARCHAR2, P_Last VARCHAR2)
	Return Varchar2;
	 Function FN_Q4
		(P_Customer NUMBER)
	Return Varchar2;
End PKG_Q4;
/
Show Errors;	
		--Package Body
Create or Replace Package Body PKG_Q4
IS	
	Function FN_Q4
		(P_First VARCHAR2, P_Last VARCHAR2)
        	Return 	VARCHAR2
	IS
	    --variables
    V_Invoice   Number (5,0);
    V_Output 	VARCHAR2(5000); 
    V_Cus       Number(6,0);
    --Cursor                    Search for customer in DB 
    Cursor C_Invoices is    Select  invoice_number
                            From    Rental_Invoice
                            Where   Customer_Number = V_Cus; 
	BEGIN
		Open    C_Invoices;
		select Customer_Number
		into V_Cus
		from Customer
		where upper(First_Name) = upper(P_First) and
				upper(Last_Name) = upper(P_Last);		
		V_Output := P_First || ', ' || P_Last || ' Customer Number: ' || V_Cus || '; ';
			Fetch C_Invoices into V_Invoice;
			If C_Invoices%NotFound 
			Then
				V_Output := V_Output || ' has no outstanding invoices...' ;            
			Else           
				While C_Invoices%Found  
				Loop               
					V_Output := V_Output || ', ' || to_char(V_Invoice);
					Fetch C_Invoices into V_Invoice;
				End Loop;
			End If;
		Close C_Invoices;
		Return V_Output;
		Exception
			WHEN 	No_Data_Found 	THEN
				Close C_Invoices;
				V_Output := P_First || ', ' || P_Last || 'does not exist in the system';
				Return V_Output;
			WHEN	Too_Many_Rows	THEN
				Close C_Invoices;
				V_Output := 'There are multiple customers with the name: ' || P_First || ', ' || P_Last || '...';       
				Return V_Output;
			WHEN    OTHERS		THEN
				Close C_Invoices;
				Raise_Application_Error(-20404,'Please contact tech support at (780)935-9404...');
	END FN_Q4;   
	
	Function FN_Q4
		(P_Customer NUMBER)
        	Return 	VARCHAR2
	IS
		--variables
		V_Invoice   Number (5,0);
		V_Output 	VARCHAR2(5000); 
		V_First		VARCHAR2(15);
		V_Last		VARCHAR2(15);
		--Cursor                    Search for customer in DB 
		Cursor C_Invoices is    Select  Invoice_Number 
								From    Rental_Invoice
								Where   P_Customer = Customer_Number; 
	BEGIN
		Open    C_Invoices;
			select First_Name, Last_Name
			into V_First,V_Last
			from Customer
			Where Customer_Number = P_Customer;			
			V_Output := V_First || ', ' || V_Last || ' Customer Number: ' || P_Customer;			
			Fetch C_Invoices into V_Invoice;
			If C_Invoices%NotFound 	Then
				V_Output := V_Output || ' has no outstanding invoices...';
			Else		
				V_Output := V_First || ', ' || V_Last || ' Customer Number: ' || P_Customer || '; ';
				While C_Invoices%Found  
				Loop               
					V_Output := V_Output || ', ' || to_char(V_Invoice);
					Fetch C_Invoices into V_Invoice;
				End Loop;
			End If;
		Close C_Invoices;
		Return V_Output;
		Exception
			WHEN 	No_Data_Found 	THEN
				Close C_Invoices;
				Raise_Application_Error(-20004, V_Output || ' does not exist in the system');
			WHEN    OTHERS	THEN
				Close C_Invoices;
				Raise_Application_Error(-20404,'Please contact tech support at (780)935-9404...');
	END FN_Q4; 
END PKG_Q4;
/
Show Errors;	

--QUESTION 5	
		--TRIGGERS
Create or Replace Trigger TR__Q5
Before insert on Rental_Invoice
For Each ROW
DECLARE	
	V_OutstandingLimit Number(2,0);
	V_OverDueLimit Number (2,0);
BEGIN
	V_OutstandingLimit = count(nvl(select Date_Returned 
						from Rental_Invoice_Detail join Rental_Invoice on 
							Rental_Invoice_Detail.Invoice_Number = Rental_Invoice.Invoice_Number
						where Rental_Invoice.Customer_Number = :New.Customer_Number and
						Rental_Invoice.Invoice_Number !=:NEW.Invoice_Number and
						Rental_Invoice_Detail.Date_Returned is null));
	V_OverDueLimit =	count(nvl(select Invoice_Date
						from Rental_Invoice_Detail join Rental_Invoice on 
							Rental_Invoice_Detail.Invoice_Number = Rental_Invoice.Invoice_Number
						where Rental_Invoice.Customer_Number = :OLD.Customer_Number	and
						Rental_Invoice.Invoice_Number !=:NEW.Invoice_Number and
						Rental_Invoice_Detail.Overdue_Paid_YN = 'N'));	
	If  V_OutstandingLimit >= 3	THEN
		RAISE_APPLICATION_ERROR(-20100,'Rental Invoice Insert failed.' || :NEW.Customer_Number ||' has Over 3 outstanding Rentals...');
	Elsif V_OverDueLimit >= 5 THEN
		RAISE_APPLICATION_ERROR(-20101,'Rental Invoice Insert failed.' || :NEW.Customer_Number ||' has Over 5 overdue Rentals...');
	End If;
END TR_Q5;
/ Show Errors;

