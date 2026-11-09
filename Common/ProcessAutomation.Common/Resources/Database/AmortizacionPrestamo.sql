IF OBJECT_ID('tempdb..#TMP_PRESTAMO') IS NOT NULL
BEGIN
	DROP TABLE #TMP_PRESTAMO
END

CREATE TABLE #TMP_PRESTAMO
(
	SecuenciaPeriodo INT,
	CantidadPeriodos INT,	
	CantidadPrestamo DECIMAL(18, 3),
	PorcentajeInteres DECIMAL(10, 3),
)

CREATE NONCLUSTERED INDEX IDX_ID_ORIGEN_SECUENCIA ON  #TMP_PRESTAMO(SecuenciaPeriodo)


INSERT INTO #TMP_PRESTAMO
SELECT 0, 12, 12000, 5

;WITH TmpAmortizacion
AS
(
	SELECT SecuenciaPeriodo AS SecuenciaPeriodo,
		CantidadPeriodos, 
		CantidadPrestamo, 
		PorcentajeInteres,
		
		SecuenciaPeriodo AS Periodo,
		CAST(0 AS DECIMAL(18,3)) AS CapitalAnterior,
		CAST(0 AS DECIMAL(18,3)) AS Interes,
		CAST(0 AS DECIMAL(18,3)) AS Capital,
		CAST(0 AS DECIMAL(18,3)) AS Quota,
		CAST(CantidadPrestamo AS DECIMAL(18,3)) AS CapitalPendiente
	FROM #TMP_PRESTAMO
	WHERE SecuenciaPeriodo = 0
	UNION ALL
	SELECT 
		SecuenciaPeriodo + 1 AS SecuenciaPeriodo, 
		CantidadPeriodos, 
		CantidadPrestamo, 
		PorcentajeInteres,
		
		SecuenciaPeriodo + 1 AS Periodo,
		capa.CapitalAnterior,
		calc.Interes AS Interes,
		cap.Capital AS Capital,
		CAST(cap.Capital + calc.Interes AS DECIMAL(18, 3)) AS Quota,
		CAST(capa.CapitalAnterior - cap.Capital AS DECIMAL(18, 3)) AS CapitalPendiente
	FROM TmpAmortizacion amo
		CROSS APPLY(
						SELECT CAST(amo.CantidadPrestamo / amo.CantidadPeriodos AS DECIMAL(18, 3)) AS Capital
					) cap
		CROSS APPLY (
						SELECT CAST(amo.CantidadPrestamo - cap.Capital * amo.SecuenciaPeriodo AS DECIMAL(18, 3)) AS CapitalAnterior
					) AS capa
		CROSS APPLY (
						SELECT CAST(capa.CapitalAnterior * (amo.PorcentajeInteres/100) AS DECIMAL(18, 3)) AS Interes
					) calc
	WHERE SecuenciaPeriodo < CantidadPeriodos
)

SELECT Periodo, Interes, Capital, Quota, CapitalPendiente
FROM TmpAmortizacion
OPTION(MAXRECURSION 0)