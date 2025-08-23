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

function ExpenseButton({ children, className, variant }) {
    variant = (variant ? "bg-" + variant : "bg-identity-dillute hover:bg-identity")
    className = variant + " " + (className || "")
    return (
        <Button className={className} >
            {children}
        </Button >
    )
}

export default async function Expenses(props) {
    const cookieStore = await cookies();
    const session = cookieStore.get("session")?.value;

    let error = null;
    let Expenses = [];
    try {
        const res = await fetch(`${process.env.NEXTJS_URL}/api/expenses`, {
            headers: {
                Cookie: `session=${session}`,
            },
            method: "GET",
            cache: 'no-store'
        });
        if (!res.ok) throw new Error('Failed to load Expenses');
        Expenses = await res.json();
    } catch (e) {
        // console.error("Failed to load Expenses,", e);
        Expenses = [];
        error = "Failed to load Expenses"
    }
    return (
        <PageLayout title="Expenses" subtitle="Generate Expenses with just a click of a button">
            <PageButton href="expenses/new">Add New Expense</PageButton>
            {

                !error ?
                    (<>
                        <Table className={"container mx-auto border-2 border-identity-dillute/20 rounded-xl "}>
                            <TableHeader >
                                <TableRow className={"bg-accent rounded-xl"}>
                                    <TH>Item</TH>
                                    <TH>Category</TH>
                                    <TH>Quantity</TH>
                                    <TH>Unit Price</TH>
                                    <TH>Receipt</TH>
                                    <TH />
                                    <TH />
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {
                                    Expenses.map((Expense, index) => (
                                        <TableRow key={index} >
                                            <Cell>
                                                {Expense.ExpenseId}
                                            </Cell>
                                            <Cell>
                                                {Expense.Category}
                                            </Cell>
                                            <Cell>
                                                {Expense.Quantity}
                                            </Cell>
                                            <Cell>
                                                {Expense.UnitPrice}
                                            </Cell>
                                            <Cell>
                                                <Link href={Expense.ReceiptUrl} className="text-identity-dillute hover:text-identity">{Expense.ReceiptUrl}</Link>
                                            </Cell>
                                            <Cell>
                                                <ExpenseButton>Edit</ExpenseButton>
                                            </Cell>
                                            <Cell>
                                                <ExpenseButton variant={"destructive"}>Delete</ExpenseButton>
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
                                    <TH>Generate Expense</TH>
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
