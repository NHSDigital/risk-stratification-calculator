export class Prediction {
  constructor() {
    this.prediction = [
      {
        outcome: {
          coding: [
            {
              code: '419620001',
              display: 'Death (event)',
              system: 'http://snomed.info/sct'
            }
          ],
          text: 'Death attributed to COVID-19',
        },
        probabilityDecimal: 0,
        relativeRisk: 0
      },
      {
        outcome: {
          coding: [
            {
              code: 'Hospitalisation',
              display: 'Hospitalisation',
              system: 'http://nhsd.nhs.uk/riskassessment',
            }
          ],
          text: 'COVID-19',
        },
        probabilityDecimal: 0,
        relativeRisk: 1
      },
      {
        outcome: {
          coding: [
            {
              code: 'BaselineDeath',
              display: 'Baseline death for age and sex',
              system: 'http://nhsd.nhs.uk/riskassessment',
            }
          ],
          text: 'Death attributed to COVID-19 for same age and sex but no other risk factors',
        },
        probabilityDecimal: 0,
        relativeRisk: 1
      },
      {
        outcome: {
          coding: [
            {
              code: 'BaselineHospitalisation',
              display: 'Baseline hospitalisation for age and sex',
              system: 'http://nhsd.nhs.uk/riskassessment',
            }
          ],
          text: 'COVID-19 for same age and sex but no other risk factors',
        },
        probabilityDecimal: 0,
        relativeRisk: 1
      },
    ];
  }

  addDeathAttributedToCovid(probabilityDecimal, relativeRisk) {
    this.prediction[0].probabilityDecimal = probabilityDecimal;
    this.prediction[0].relativeRisk = relativeRisk;
  }

  addHospitalisation(probabilityDecimal, relativeRisk) {
    this.prediction[1].probabilityDecimal = probabilityDecimal;
    this.prediction[1].relativeRisk = relativeRisk;
  }

  addBaselineDeath(probabilityDecimal) {
    this.prediction[2].probabilityDecimal = probabilityDecimal;
    // this.prediction.outcome[2].relativeRisk = 1;
  }

  addBaselineHospitalisation(probabilityDecimal) {
    this.prediction[3].probabilityDecimal = probabilityDecimal;
    // this.prediction.outcome[3].relativeRisk = 1;
  }
}
