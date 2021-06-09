import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age, sex, height, weight, asthma, ethnicGroup
} from '../../../helpers/snomedCodes';
import {
  chemotherapy
} from '../../../helpers/qCovidCodes';
import * as OperationOutcomeDiagnostics from '../../../helpers/observationOutcomeDiagnostics';

describe('It should get an Operation Outcome', () => {
  it('It should fail if not all compulsory fields are generated', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.issue[0].diagnostics)
      .toContain(OperationOutcomeDiagnostics.MISSING_REQUIRED_OBSERVATION_QCOVID);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should fail if wrong snomed code sent', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, asthma.snomedCode, 29, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.issue[0].diagnostics)
      .toContain(OperationOutcomeDiagnostics.MISSING_REQUIRED_OBSERVATION_QCOVID);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should fail if same observation is sent twice', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 29, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 100.7, weight.unit);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);
    expect(response.issue[0].diagnostics)
      .toContain(OperationOutcomeDiagnostics.DUPLICATED_OBSERVATION);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should fail if two chemotherapy of the same type are sent', async () => {
    const bundle = new CreateBundle();
    bundle.addObservationValueQuantity(age.id, age.snomedCode, 29, age.unit);
    bundle.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundle.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundle.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundle.addObservationValueCodeableConcept(chemotherapy.id, chemotherapy.qcovidCode,
      chemotherapy.imids.qcovidCode);
    bundle.addObservationValueCodeableConcept(chemotherapy.id, chemotherapy.qcovidCode,
      chemotherapy.ifophosphamide.qcovidCode);
    const response = await client.postError(bundle.bundle);
    expect(response.issue[0].diagnostics)
      .toContain(OperationOutcomeDiagnostics.DUPLICATED_OBSERVATION);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should fail if two chemotherapy of the different types are sent', async () => {
    const bundle = new CreateBundle();
    bundle.addObservationValueQuantity(age.id, age.snomedCode, 29, age.unit);
    bundle.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundle.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundle.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundle.addObservationValueCodeableConcept(chemotherapy.id, chemotherapy.qcovidCode,
      chemotherapy.imids.qcovidCode);
    bundle.addObservationValueCodeableConcept(chemotherapy.id, chemotherapy.qcovidCode,
      chemotherapy.lenvatinib.qcovidCode);
    const response = await client.postError(bundle.bundle);
    expect(response.issue[0].diagnostics)
      .toContain(OperationOutcomeDiagnostics.DUPLICATED_OBSERVATION);
    expect(response.issue[0].severity).toBe('error');
  });

  it('It should fail if wrong json code formed', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 29, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode, sex.female.snomedCode);
    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86.7, weight.unit);
    bundleCreator.addObservationWrongJsonFormat(asthma.id, asthma.snomedCode, true);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianBangladeshi.snomedCode);

    const response = await client.postError(bundleCreator.bundle);

    expect(response.message)
      .toContain(OperationOutcomeDiagnostics.INVALID_REQUEST_BODY);
  });
});
