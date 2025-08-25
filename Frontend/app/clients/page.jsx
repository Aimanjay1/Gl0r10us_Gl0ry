import { PageLayout, PageButton, TH, Cell } from "@/components/PageCommon";
import {
    Table,
    TableBody,
    TableCaption,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import { cookies } from "next/headers";
import { useUser } from "@/components/UserProvider";


export default async function Clients(props) {
    const cookieStore = await cookies();
    const session = cookieStore.get("session")?.value;

    let error;


    let Clients = [];
    try {
        const res = await fetch(`${process.env.NEXTJS_URL}/api/clients`, {
            headers: {
                Cookie: `session=${session}`,
            },
            cache: "no-store",
        });
        // if (!res.ok) throw new Error('Failed to load Clients');
        if (res.ok)
            Clients = await res.json();
        else
            error = "Failed to load clients"
    } catch (e) {
        // console.error("Failed to load Clients,", e);
        Clients = [];
    }
    return (
        <PageLayout title="Clients">
            <PageButton href="clients/new">Add New Client</PageButton>

            {
                !error ?
                    (<>
                        {
                            Clients.length > 0 || <div className="container mx-auto">No clients has been made</div>
                        }
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <colgroup><col className="w-32" /><col className="w-32" /><col className="w-28" /><col className="w-20" /><col className="w-28" /></colgroup>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customer</TH>
                                    <TH>Contact Number</TH>
                                    <TH>Company Name</TH>
                                    <TH>Company Address</TH>
                                    <TH>Email</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {
                                    Clients.map((Client, index) => (
                                        <TableRow key={index} >
                                            <Cell>
                                                <div className="flex flex-col max-w-[200px] mx-auto justify-center">
                                                    <h1 className="font-bold w-full text-ellipsis overflow-hidden">{Client.clientName}</h1>
                                                    <p className="">{Client.clientId}</p>
                                                </div>
                                            </Cell>
                                            <Cell>
                                                010tekan2xdpt
                                            </Cell>
                                            <Cell>
                                                {Client.companyName}
                                            </Cell>
                                            <Cell>
                                                {Client.CompanyAddress}
                                            </Cell>
                                            <Cell>
                                                {Client.email}
                                            </Cell>
                                        </TableRow>
                                    ))
                                }


                            </TableBody>

                        </Table>
                    </>)
                    :
                    (<>
                        <Badge variant={"destructive"} className={"mx-auto"}>{error}</Badge>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Customers</TH>
                                    <TH>Status</TH>
                                    <TH>Order Date

                                    </TH>
                                    <TH>Due Date</TH>
                                    <TH>Generate Client</TH>
                                    <TH>Send an email</TH>
                                    <TH>Receipt</TH>
                                </TableRow>
                            </TableHeader>
                            <TableBody></TableBody>
                        </Table>
                    </>)
            }

        </PageLayout>
    )
}