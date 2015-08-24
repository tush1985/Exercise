
--Query 1  

--	Write a query that retrieves the total amount of paid commission for a specific dealer over a custom time period.

declare @dealerid int
set @dealerid=2

select creditamount.dealerid,(creditamount.camount-refundamount.ramount) commission from
(select d.dealerid,sum(ch.newcommissionamount) camount from dealer d inner join commission c on d.dealerid= c.dealerid
inner join commissionhistory ch on c.commissionid=ch.commissionid 
and ch.newpaymentstatusid=5 
and ch.newdealerid=@dealerid 
and d.dealerid=@dealerid
group by(d.dealerid)) creditamount 

left join
 
(select de.dealerid, ISNULL( sum(ch.newcommissionamount),0) ramount from dealer de 
inner join commission c on de.dealerid= c.dealerid
left join commissionhistory ch on c.commissionid=ch.commissionid 
and ch.newpaymentstatusid=6
and ch.newdealerid=@dealerid 
and de.dealerid=@dealerid
group by(de.dealerid) )
 refundamount  
on creditamount.dealerid=refundamount.dealerid


-----------Query 1 Ended

---Query 1 OutPut--------------
DealerID            Commission
2                     560.11
-------------------------------


--Query 2

--  Write a query to calculate the average period (in days) all commissions have taken to transition from ‘Verified’ to ‘Paid’ for a single dealer.

declare @dealerid2 int
set @dealerid2= 3
select AVG( z.noofdays)  AverageDays
from dealer d inner join commission c on d.dealerid=c.dealerid
inner join 
(select a.commissionid,a.paid,b.verified,datediff(DAY,b.verified,a.paid) noofdays   from
(select commissionid,actiondate paid from commissionhistory where newpaymentstatusid=5 and commissionid in 
(select commissionid from commissionhistory where newpaymentstatusid=5)) a
inner join 
(select commissionid ,actiondate verified from commissionhistory where newpaymentstatusid=2 and commissionid in 
(select commissionid from commissionhistory where newpaymentstatusid=5) ) b
on a.commissionid = b.commissionid) z
on c.commissionid=z.commissionid
and d.dealerid=@dealerid2

-----------End--------------
---Query 2 OutPut--------------
AverageDays
33
-------------------------------


--Query 2--Redesign
----  Write a query to calculate the average period (in days) all commissions have taken to transition from ‘Verified’ to ‘Paid’ for a single dealer.

declare @dealerid2 int
set @dealerid2= 2
select avg (DATEDIFF(DAY,b.verified,a.paid)) AverageDays from
(select  ch.actiondate paid ,c.commissionid from commission c
inner join commissionhistory ch on c.commissionid=ch.commissionid
inner join dealer d on c.dealerid=d.dealerid and 
d.dealerid=@dealerid2
and c.paymentstatusid>=5 and ch.newpaymentstatusid=5)
a inner join
(select  ch.actiondate verified ,c.commissionid from commission c
inner join commissionhistory ch on c.commissionid=ch.commissionid
inner join dealer d on c.dealerid=d.dealerid
and 
d.dealerid=@dealerid2
and c.paymentstatusid>=5 and ch.newpaymentstatusid=2)
b on a.commissionid=b.commissionid

-----------End Query 2-----------
---RQuery 2--Redesign  OutPut--------------
AverageDays
12
-------------------------------

---Query 3-----

--	Write a query to calculate how many levels of the dealer hierarchy each dealer is from the highest level.

declare @dealerid3 int
set @dealerid3 =5
;with MyCTE
as (select d1.dealerid,d1.name,d1.parentdealerid,[level]=0 from dealer d1
where dealerid=@dealerid3
union all 
select d2.dealerid,d2.name,d2.parentdealerid,[level]+1 from dealer d2
inner join MyCTE on d2.dealerid=MyCTE.parentdealerid
)
select * from MyCTE d3
where
d3.[level]=(select MAX([level]) from MyCTE)

-----------Query End-----

--Query 3 OutPut------------------------------
DealerID     Name           parentdealerid     level
1	         TestDealer1	NULL	             3
------------------------------------------------



---Stored Procedure for the CommissionReport

USE [Marketing]
GO
/****** Object:  StoredProcedure [dbo].[GetCommissionReport]    Script Date: 08/24/2015 11:28:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Tushar Trivedi
-- Description: Get the Commission data based on Admin role or dealer role
-- =============================================
ALTER PROCEDURE [dbo].[GetCommissionReport]
( 
	-- Add the parameters for the stored procedure here
	@IsAdmin bit,	
	@UserName nvarchar(50),
	@StartDate DateTime = null,
	@EndDate   DateTime = null,
	@Status int=0
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--if an admin run the report
	if @IsAdmin = 1
	begin	
	--if startdate and enddate is not null, get the commission value in between the startdate and enddate
		if @StartDate is not null and @EndDate is not null
		begin
		select d.DealerId,d.Name,c.CommissionID,c.CustomerId,
		c.ProductId,c.CommissionAmount,c.PaymentStatusId,c.CreatedDate,c.ModifiedDate
		from dealer d 
		inner join commission c on d.dealerid= c.dealerid
		and c.CreatedDate>=@StartDate and c.CreatedDate<=@EndDate
		print 'Admin startdate and enddaate' ;
		end
		---if admin wants to see a commission data based on Paymentstatus
		else if @Status>0
		begin
		select d.DealerId,d.Name,c.CommissionID,c.CustomerId,
		c.ProductId,c.CommissionAmount,c.PaymentStatusId,c.CreatedDate,c.ModifiedDate
		from dealer d 
		inner join commission c on d.dealerid= c.dealerid
		and c.PaymentStatusId=@Status		
		print 'Admin paymentstatus' ;
		end
		--default when admin login it will goes to this part
		else
		begin 	
		select d.DealerId,d.Name,c.CommissionID,c.CustomerId,
		c.ProductId,c.CommissionAmount,c.PaymentStatusId,c.CreatedDate,c.ModifiedDate
		from dealer d 
		inner join commission c on d.dealerid= c.dealerid		
		print 'Admin default'
		end
	end
	else--If not an admin user
	begin
		--if startdate and enddate is not null, get the commission value in between the startdate and enddate
		if @StartDate is not null and @EndDate is not null
		begin
		select d.DealerId,d.Name,c.CommissionID,c.CustomerId,
		c.ProductId,c.CommissionAmount,c.PaymentStatusId,c.CreatedDate,c.ModifiedDate
		from dealer d 
		inner join commission c on d.dealerid= c.dealerid
		and d.Name = @UserName
		and c.CreatedDate>=@StartDate and c.CreatedDate<=@EndDate			
		print 'dealer startdate and enddaate' ;	
		end
	  ---if admin wants to see a commission data based on Paymentstatus
		else if @Status>0
		begin
		select d.DealerId,d.Name,c.CommissionID,c.CustomerId,
		c.ProductId,c.CommissionAmount,c.PaymentStatusId,c.CreatedDate,c.ModifiedDate
		from dealer d 
		inner join commission c on d.dealerid= c.dealerid
		and d.Name = @UserName
		and c.PaymentStatusId=@Status		
		print 'dealer paymentstatus' ;		
		end
		else--default when dealer login it will goes to this part
		begin 		
		select d.DealerId,d.Name,c.CommissionID,c.CustomerId,
		  c.ProductId,c.CommissionAmount,c.PaymentStatusId,c.CreatedDate,c.ModifiedDate
		  from dealer d 
		  inner join commission c on d.dealerid= c.dealerid
		  and d.Name = @UserName
		  	print 'Dealer default'
		  end
	end
	
END








