using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Contract;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessAutomation.Component
{
    public class EconomyComponent : TaskComponentBaseBC, ITaskComponent
    {
        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            return true;
        }

        public void EndComponent()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        private List<Dictionary<string, object>> GetSupplyDemandDataList(string sourceBy, string fromTable, string fromQuery, string relationalField, string keyField, string quantityField, string orderBy)
        {
            List<Dictionary<string, object>> dataList = null;
            if (!string.IsNullOrWhiteSpace(sourceBy))
            {
                string query = null;
                switch (sourceBy)
                {
                    case "TABLE":
                        {
                            query = string.Format("SELECT {0}, {1}, {2} FROM {3} ", relationalField, keyField, quantityField, fromTable);

                            if (!string.IsNullOrWhiteSpace(orderBy))
                            {
                                query = string.Format("{0} ORDER BY {1}, {2}", query, relationalField, orderBy);
                            }
                            else
                            {
                                query = string.Format("{0} ORDER BY {1}", query, relationalField);
                            }
                        }
                        break;

                    case "QUERY":
                        query = fromQuery.Trim();
                        break;
                }

                WriteMessageLog($"Query: {query}");

                if (query != null)
                {
                    IDAC dac = processBC.GetDatabaseDAC();
                    dataList = dac.RetrieveListByQuery(query, 0, null);

                    if (dataList == null || dataList.Count == 0)
                    {
                        WriteMessageLog($"0 regedits");
                    }
                }
            }

            return dataList;
        }

        private decimal SetPrecision(object value, string decimalOption, int decimalQuantity)
        {
            return SetPrecision(Convert.ToDecimal(value), decimalOption, decimalQuantity);
        }

        private decimal SetPrecision(decimal value, string decimalOption, int decimalQuantity)
        {
            if (decimalQuantity >= 0)
            {
                switch (decimalOption)
                {
                    case "ROUND":
                        value = Math.Round(value, decimalQuantity);
                        break;

                    case "TRUNCATE":
                        {
                            decimalQuantity = int.Parse(decimalQuantity.ToString().PadRight(decimalQuantity, '0'));
                            value = Math.Truncate(value * decimalQuantity) / decimalQuantity;
                        }
                        break;
                }
            }

            return value;
        }

        private string SetStringFormat(object value, string decimalOption, int decimalQuantity)
        {
            string stringResult = null;

            if (decimalQuantity > 0)
            {
                switch (decimalOption)
                {
                    case "ROUND":
                    case "TRUNCATE":
                        {
                            stringResult = string.Format($"{{0:#.{decimalQuantity}}}", value.ToString());
                        }
                        break;
                }
            }
            else
            {
                stringResult = value.ToString();
            }

            return stringResult;
        }

        private TableType ProcessSupplyAndDemand()
        {
            TableType tableTypeResult = null;
            EconomyDTO dto = processBC.GetProcessedInputValue<EconomyDTO>();

            if (dto != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.DemandSourceBy))
                {
                    List<SupplyDemandDTO> distributorList = new List<SupplyDemandDTO>();

                    dto.DecimalOption = dto.DecimalOption?.Trim();
                    dto.DecimalQuantity = dto.DecimalQuantity <= 0 ? 0 : dto.DecimalQuantity;

                    dto.SupplySourceBy = dto.SupplySourceBy?.Trim();
                    dto.SupplyTable = dto.SupplyTable?.Trim();
                    dto.SupplyQuery = dto.SupplyQuery?.Trim();
                    dto.SupplyRelationalField = dto.SupplyRelationalField?.Trim();
                    dto.SupplyKeyField = dto.SupplyKeyField?.Trim();
                    dto.SupplyQuantityField = dto.SupplyQuantityField?.Trim();
                    dto.SupplyOrderBy = dto.SupplyOrderBy?.Trim();

                    dto.DemandSourceBy = dto.DemandSourceBy?.Trim();
                    dto.DemandTable = dto.DemandTable?.Trim();
                    dto.DemandQuery = dto.DemandQuery?.Trim();
                    dto.DemandRelationalField = dto.DemandRelationalField?.Trim();
                    dto.DemandKeyField = dto.DemandKeyField?.Trim();
                    dto.DemandQuantityField = dto.DemandQuantityField?.Trim();
                    dto.DemandOrderBy = dto.DemandOrderBy?.Trim();

                    List<Dictionary<string, object>> supplyList = GetSupplyDemandDataList(dto.SupplySourceBy, dto.SupplyTable, 
                        dto.SupplyQuery, dto.SupplyRelationalField, dto.SupplyKeyField, dto.SupplyQuantityField, dto.SupplyOrderBy);

                    List<Dictionary<string, object>> demandList = GetSupplyDemandDataList(dto.DemandSourceBy, dto.DemandTable,
                        dto.DemandQuery, dto.DemandRelationalField, dto.DemandKeyField, dto.DemandQuantityField, dto.DemandOrderBy);

                    if (supplyList != null && demandList != null && supplyList.Any() && demandList.Any())
                    {
                        long sequence = 0;
                        decimal demandRemainingQuantity = 0;
                        decimal supplyRemainingQuantity = 0;
                        decimal assignedQuantity = 0;
                        string currentKey = null;
                        decimal demandQuantity = 0;
                        decimal supplyQuantity = 0;

                        foreach (Dictionary<string, object> demandData in demandList.OrderBy(o => o[dto.DemandRelationalField]))
                        {
                            if (currentKey != demandData[dto.DemandRelationalField].ToString())
                            {
                                currentKey = demandData[dto.DemandRelationalField].ToString();
                                sequence = 0;
                            }

                            demandQuantity = SetPrecision(demandData[dto.DemandQuantityField], dto.DecimalOption, dto.DecimalQuantity);
                            demandRemainingQuantity = demandQuantity;

                            foreach (Dictionary<string, object> supplyData in
                                supplyList.Where(f => Convert.ToString(f[dto.SupplyRelationalField]) == Convert.ToString(demandData[dto.DemandRelationalField])
                                            && Convert.ToDecimal(f[dto.SupplyQuantityField]) > 0))
                            {
                                supplyQuantity = SetPrecision(supplyData[dto.SupplyQuantityField], dto.DecimalOption, dto.DecimalQuantity);

                                supplyRemainingQuantity = supplyQuantity;

                                demandRemainingQuantity -= supplyQuantity;
                                demandRemainingQuantity = demandRemainingQuantity <= 0 ? 0 : demandRemainingQuantity;

                                supplyRemainingQuantity -= demandQuantity;
                                supplyRemainingQuantity = supplyRemainingQuantity <= 0 ? 0 : supplyRemainingQuantity;

                                assignedQuantity = supplyQuantity - supplyRemainingQuantity;

                                distributorList.Add(new SupplyDemandDTO
                                {
                                    Sequence  = ++sequence,
                                    RelationalKey = supplyData[dto.SupplyRelationalField].ToString(),
                                    DemandKey = demandData[dto.DemandKeyField].ToString(),
                                    DemandQuantity = demandQuantity,
                                    DemandRemainingQuantity = demandRemainingQuantity,
                                    SupplyKey = supplyData[dto.SupplyKeyField].ToString(),
                                    SupplyQuantity = supplyQuantity,
                                    SupplyRemainingQuantity = supplyRemainingQuantity,
                                    AssignedQuantity = assignedQuantity
                                });

                                demandData[dto.DemandQuantityField] = demandQuantity = demandRemainingQuantity;
                                supplyData[dto.SupplyQuantityField] = supplyQuantity = supplyRemainingQuantity;

                                if (demandRemainingQuantity <= 0)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    tableTypeResult = new TableType(DataSerialiazerManager.DeserializerToList(JsonConvert.SerializeObject(distributorList)));
                }
            }

            return tableTypeResult;
        }

        private TableType ProcessAmortization()
        {
            TableType tableTypeResult = null;
            EconomyDTO dto = processBC.GetProcessedInputValue<EconomyDTO>();

            if (dto != null && dto.InterestRate > 0 && dto.LoanAmount > 0 && dto.NumberPeriods > 0)
            {
                dto.DecimalOption = dto.DecimalOption?.Trim();
                dto.DecimalQuantity = dto.DecimalQuantity <= 0 ? 0 : dto.DecimalQuantity;

                List<AmortizationDTO> amortizationList = new List<AmortizationDTO>();

                dto.LoanAmount = SetPrecision(dto.LoanAmount, dto.DecimalOption, dto.DecimalQuantity);
                int counterPeriod = 0;
                decimal interestRate = 0;
                decimal capital = 0;
                decimal interest = 0;
                decimal fee = 0;
                decimal remainigCapital = dto.LoanAmount;

                while (counterPeriod <= dto.NumberPeriods)
                {   
                    if (counterPeriod > 0)
                    {
                        interestRate = dto.InterestRate;
                        remainigCapital = SetPrecision(remainigCapital, dto.DecimalOption, dto.DecimalQuantity);

                        interest = remainigCapital * (dto.InterestRate / 100);
                        capital = SetPrecision(dto.LoanAmount / dto.NumberPeriods, dto.DecimalOption, dto.DecimalQuantity);
                        fee = interest + capital;
                    }

                    remainigCapital -= capital;

                    amortizationList.Add(new AmortizationDTO
                    {
                        Period = counterPeriod,
                        InterestRate = interestRate,
                        Interest = interest,
                        Capital = capital,
                        Fee = fee,
                        RemainingCapital = remainigCapital
                    });

                    counterPeriod++;
                }

                tableTypeResult = new TableType(DataSerialiazerManager.DeserializerToList(JsonConvert.SerializeObject(amortizationList)));
            }

            return tableTypeResult;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            IObjectType result = null;
            try
            {
                EconomyDTO dto = processBC.GetProcessedInputValue<EconomyDTO>();

                switch (dto.EconomyType)
                {
                    case "AMORTIZATION":
                        result = ProcessAmortization();
                        break;

                    case "SUPPLY_AND_DEMAND":
                        result = ProcessSupplyAndDemand();
                        break;
                }

                StepSuccess();
            }
            catch (Exception ex)
            {
                StepFail(ex.ToString());
            }
            return result;
        }
    }
}