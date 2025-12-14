import type { Severity } from "@/types/packages";
import { severityToBgColor, severityToBorderColor } from "@/utils/riskToColor";
import RadioSelect from "../../RadioSelect";
import { useLazyGetPackageQuery } from "@/store/api/projectsApi";
import type { VulnerabilityDetailedDto } from "@/types/vulnerabilities";
import { useState, useEffect } from "react";
import { Loader } from "@/components/chart/PieCustomChart";

const Information = ({ packageId }: { packageId?: string }) => {
  const [fetch, { isFetching, data }] = useLazyGetPackageQuery();
  const [vulnerability, setVulnerablity] = useState<
    VulnerabilityDetailedDto | undefined
  >();

  useEffect(() => {
    async function fetchData() {
      if (packageId) {
        const result = await fetch(packageId);

        if (result.data && result.data.vulnerabilities.length > 0) {
          setVulnerablity(result.data.vulnerabilities[0]);
        }
      }
    }
    fetchData();
  }, [packageId, fetch]);

  if (isFetching) {
    return <Loader />;
  }

  if (!data) {
    return <div className="self-center">No data available</div>;
  }

  return (
    <div className="text-xl">
      <p>
        <strong>Name: </strong>
        {data.name}
      </p>
      <p>
        <strong>Version: </strong>
        {data.version}
      </p>
      <p>
        <strong>Ecosystem: </strong>
        {data.ecosystem}
      </p>
      {data.vulnerabilities.length === 0 ? (
        <p className="pt-4 self-center w-full text-center">
          This package has no known vulnerabilities
        </p>
      ) : (
        <div className="pt-4">
          <div className="flex flex-col gap-4 items-center">
            <strong>Vulnerabilities</strong>
            <div className="flex flex-row gap-2">
              {data.vulnerabilities.map((vuln) => (
                <RadioSelect
                  onClick={() => setVulnerablity(vuln)}
                  bgColor={severityToBgColor[vuln.severity as Severity]}
                  borderColor={severityToBorderColor[vuln.severity as Severity]}
                  key={vuln.id}
                  selected={vuln.id === vulnerability?.id}
                />
              ))}
            </div>
          </div>
          <div className="flex flex-col items-start w-full pt-2">
            <p>
              <strong>Name: </strong>
              {vulnerability?.id}
            </p>
            <p>
              <strong>Severity: </strong>
              {vulnerability?.severity}
            </p>
            <p>
              <strong>Recommendation: </strong>
              {vulnerability?.recommendation}
            </p>
            <p className="overflow-auto max-h-50 h-60">
              <strong>Description: </strong>
              {vulnerability?.description}
            </p>
          </div>
        </div>
      )}
    </div>
  );
};

export default Information;
