USE Programacion_Perfil

IF OBJECT_ID('tempdb..#TMP_DEMANDA') IS NOT NULL
BEGIN
	DROP TABLE #TMP_DEMANDA
END


IF OBJECT_ID('tempdb..#TMP_OFERTA') IS NOT NULL
BEGIN
	DROP TABLE #TMP_OFERTA
END


CREATE TABLE #TMP_DEMANDA
(
	SecuenciaDemanda INT,
	IdLlave VARCHAR(50),
	IdDemanda VARCHAR(50),
	CantidadDemanda DECIMAL(18, 3)
)

CREATE NONCLUSTERED INDEX IDX_ID_ORIGEN_SECUENCIA ON  #TMP_DEMANDA(IdLlave, SecuenciaDemanda)
CREATE NONCLUSTERED INDEX IDX_ID_ORIGEN_NECESIDAD ON  #TMP_DEMANDA(IdLlave, IdDemanda)

CREATE TABLE #TMP_OFERTA
(
	SecuenciaOferta INT,
	IdLlave VARCHAR(50),
	IdOferta VARCHAR(50),
	CantidadOferta DECIMAL(18, 3)
)
CREATE NONCLUSTERED INDEX IDX_ID_ORIGEN_SECUENCIA ON  #TMP_OFERTA(IdLlave, SecuenciaOferta)
CREATE NONCLUSTERED INDEX IDX_ID_ORIGEN_OFERTA ON  #TMP_OFERTA(IdLlave, IdOferta)


INSERT INTO #TMP_DEMANDA
SELECT DISTINCT
		ROW_NUMBER() OVER (PARTITION BY CONCAT(pro.Linea, '-', pro.Programa) ORDER BY pro.CantidadProgramada DESC) AS SecuenciaDemanda,
		CONCAT(pro.Linea, '-', pro.Programa) AS IdLlave, pro.Necesidad, CAST(pro.CantidadProgramada AS DECIMAL(18, 3)) AS CantidadDemanda
FROM P_DWH.dbo.Programa_Detalle_DWH_Refresh pro
GROUP BY pro.Linea, pro.Programa, pro.Necesidad, pro.CantidadProgramada 

INSERT INTO #TMP_OFERTA
SELECT 
	ROW_NUMBER() OVER (PARTITION BY CONCAT(pro.Linea, '-', pro.Programa) ORDER BY sto.PesoNeto ASC) AS SecuenciaOferta,
	CONCAT(pro.Linea, '-', pro.Programa) AS IdLlave, pro.Rollo, CAST((sto.PesoNeto/1000) AS DECIMAL(18, 3)) AS CantidadOferta
FROM P_DWH.dbo.Programa_Detalle_DWH_Refresh pro
	INNER JOIN p_dwh.dbo.Stock_DWH_Refresh sto ON pro.Rollo = sto.IdMaterial
WHERE pro.Linea IN ('C3_CSIN', 'C4_CSIN', 'CTL3_CHU', 'CL2_CSIS')
GROUP BY pro.Linea, pro.Programa, pro.Rollo, sto.PesoNeto


;WITH TmpDistribucion
AS
(
	SELECT 
		CASE WHEN dem.CantidadDemanda - ofe.CantidadOferta > 0 THEN dem.SecuenciaDemanda ELSE dem.SecuenciaDemanda+1 END AS SecuenciaDemanda,
		dem.IdLlave, dem.IdDemanda, 
		dem.CantidadDemanda AS CantidadDemanda,
		CAST((dem.CantidadDemanda - ofe.CantidadOferta) AS DECIMAL(18,3)) AS CantidadDemandaRestante,
		CASE WHEN ofe.CantidadOferta - dem.CantidadDemanda > 0 THEN ofe.SecuenciaOferta ELSE ofe.SecuenciaOferta+1 END AS SecuenciaOferta, 
		ofe.IdOferta, 
		CAST(ofe.CantidadOferta AS DECIMAL(18,3)) AS CantidadOferta,
		CAST((ofe.CantidadOferta - dem.CantidadDemanda)  AS DECIMAL(18,3)) AS CantidadOfertaRestante,
		CAST((CASE WHEN dem.CantidadDemanda > ofe.CantidadOferta THEN ofe.CantidadOferta ELSE dem.CantidadDemanda END ) AS DECIMAL(18, 3)) AS CantidadAsignada
	FROM #TMP_DEMANDA dem 
		INNER JOIN #TMP_OFERTA ofe ON dem.IdLlave = ofe.IdLlave 
	WHERE dem.SecuenciaDemanda = 1
		AND ofe.SecuenciaOferta = 1
	UNION ALL
	SELECT
		CASE WHEN cant.CantidadDemandaRestante - cant.CantidadOfertaRestante > 0 THEN dis.SecuenciaDemanda ELSE dis.SecuenciaDemanda+1 END AS SecuenciaDemanda, 
		dem.IdLlave, dem.IdDemanda, cant.CantidadDemandaRestante AS CantidadDemanda,
		CAST((cant.CantidadDemandaRestante - cant.CantidadOfertaRestante) AS DECIMAL(18,3)) AS CantidadDemandaRestante,
		CASE WHEN cant.CantidadOfertaRestante - cant.CantidadDemandaRestante > 0 THEN dis.SecuenciaOferta ELSE dis.SecuenciaOferta+1 END AS SecuenciaOferta, 
		ofe.IdOferta, 
		CAST(cant.CantidadOfertaRestante AS DECIMAL(18, 3)) AS CantidadOferta,
		CAST((cant.CantidadOfertaRestante - cant.CantidadDemandaRestante)  AS DECIMAL(18,3)) AS TnsRestantes,
		CAST((CASE WHEN cant.CantidadDemandaRestante > cant.CantidadOfertaRestante THEN cant.CantidadOfertaRestante ELSE cant.CantidadDemandaRestante END ) AS DECIMAL(18, 3)) AS CantidadAsignada
	FROM TmpDistribucion dis
		INNER JOIN #TMP_DEMANDA dem ON dis.IdLlave = dem.IdLlave AND ((dis.IdDemanda = dem.IdDemanda AND dis.CantidadDemandaRestante > 0) OR (dis.SecuenciaDemanda = dem.SecuenciaDemanda))
		INNER JOIN #TMP_OFERTA ofe ON dis.IdLlave = ofe.IdLlave AND ((dis.IdOferta = ofe.IdOferta AND dis.CantidadOfertaRestante > 0) OR (dis.SecuenciaOferta = ofe.SecuenciaOferta))
		CROSS APPLY (SELECT 
						CASE WHEN dis.CantidadOfertaRestante > 0 THEN dis.CantidadOfertaRestante ELSE ofe.CantidadOferta END AS CantidadOfertaRestante,
						CASE WHEN dis.CantidadDemandaRestante > 0 THEN dis.CantidadDemandaRestante ELSE dem.CantidadDemanda END AS CantidadDemandaRestante
					 ) as cant
)

SELECT
	ROW_NUMBER() OVER (PARTITION BY IdLlave ORDER BY SecuenciaDemanda, SecuenciaOferta ASC) AS Secuencia,
	IdLlave, IdDemanda, CantidadDemanda, 
	CASE WHEN CantidadDemandaRestante > 0 THEN CantidadDemandaRestante ELSE 0 END CantidadDemandaRestante, 
	IdOferta, CantidadOferta, 
	CASE WHEN CantidadOfertaRestante > 0 THEN CantidadOfertaRestante ELSE 0 END CantidadOfertaRestante, 
	CantidadAsignada
FROM TmpDistribucion
--WHERE IdLlave = 'C3_CSIN-230689'
ORDER BY IdLlave, Secuencia
OPTION(MAXRECURSION 0)