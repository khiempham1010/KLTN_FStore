alter PROC GetRevenueDaily
	@fromDate VARCHAR(10),
	@toDate VARCHAR(10)
AS
BEGIN
		select
                CAST(b.DateCreated AS DATE) as "Date",
                SUM(bd.Quantity*bd.Price) as Revenue,
                SUM((bd.Quantity*bd.Price)-(bd.Quantity * p.OriginalPrice)) as Profit
                from 
					Orders b inner join dbo.OrderDetails bd
					on b.Id = bd.OrderId inner join Products p
					on bd.ProductId  = p.Id

                where b.DateCreated <= cast(@toDate as date) 
						AND b.DateCreated >= cast(@fromDate as date)
						AND b.OrderStatus != 3
                group by CAST(b.DateCreated AS DATE)
END
go



EXEC dbo.GetRevenueDaily @fromDate = '6/20/2020',
                         @toDate = '6/26/2020' 
select *
from Visitors