import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age,
  sex,
  ethnicGroup,
  height,
  weight,
  housing,
} from '../../../helpers/snomedCodes';
import {
  chemotherapy
} from '../../../helpers/qCovidCodes';
import { Prediction } from '../../../helpers/riskAssessment_Prediction';

describe('The received chemotherapy will have an impact on the risk score', () => {
  function createPatient(chemo) {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 26, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.male.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.anyOtherGroup.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 160, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 80, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id, chemotherapy.qcovidCode,
      chemo);
    return bundleCreator;
  }

  it('Score for no chemotherapy', async () => {
    const bundle = createPatient(chemotherapy.none.qcovidCode);

    const response = await client.postSuccess(bundle.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.000833, 3.685840707964602);
    expectedPred.addHospitalisation(0.038412, 4.113074204946996);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for SACT_CHEMOTHERAPY_A type chemotherapy', async () => {
    const bundle = createPatient(chemotherapy.btkInhibitors.qcovidCode);

    const response = await client.postSuccess(bundle.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.002913, 12.889380530973451);
    expectedPred.addHospitalisation(0.139824, 14.972052682300031);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for SACT_CHEMOTHERAPY_B type chemotherapy', async () => {
    const bundle = createPatient(chemotherapy.cabazitaxel.qcovidCode);

    const response = await client.postSuccess(bundle.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.002913, 12.889380530973451);
    expectedPred.addHospitalisation(0.139824, 14.972052682300031);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for SACT_CHEMOTHERAPY_C type chemotherapy', async () => {
    const bundle = createPatient(chemotherapy.allAll_amlRegimens.qcovidCode);

    const response = await client.postSuccess(bundle.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundle.bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.002803, 12.402654867256636);
    expectedPred.addHospitalisation(0.15767, 16.882963914766034);
    expectedPred.addBaselineDeath(0.000226);
    expectedPred.addBaselineHospitalisation(0.009339);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });
});
